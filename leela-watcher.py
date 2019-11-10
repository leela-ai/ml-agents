#  Agent which communicates with the Leela Brain to learn
#  Communicates with the Leela Brain via WAMP
#  Currently supports "capabilities" command, and "step" command.
#  Connects to Machine Learning in Unity Toolkit

# These commands are currently supported
#### Used by brain to control sms
# get_capabilities() returns {"sensors": {"items": {"hp11":false...}} {"actions":"handr",....}}
# step_world(action_list) returns {"items":{"hp11":false...}} {"actions":"handr",....} {"debuginfo":....}}

#### Used by test client to do things in the sms
# get_from_sms()
# do_in_sms()
# include standard modules

#from autobahn.asyncio.wamp import ApplicationSession, ApplicationRunner
#from autobahn.wamp.types import RegisterOptions
#from autobahn.asyncio.component import Component, run

import asyncio
from os import environ
from autobahn.asyncio.wamp import ApplicationSession, ApplicationRunner
from autobahn.wamp.types import PublishOptions, EventDetails, SubscribeOptions

import json

from math import ceil
import ssl
import sys

from mlagents.envs import UnityEnvironment
import argparse


print (sys.argv)

# initiate the parser
parser = argparse.ArgumentParser(description='Leela Crossbar agent for Unity ML Agents worlds')

parser.add_argument("-g", "--gridsize", help="Blocksworld grid size",)
parser.add_argument("-W", "--world", help="Choose a prebuilt Unity world to run from envs/ directory",)
parser.add_argument("-d", "--debug", help="Enable debug printing", action="store_true")
parser.add_argument("-I", "--interactive", help="Run Unity game interactively", action="store_true")


cmdline_args = parser.parse_args()

# default to interactive launch of game script from Unity
unity_world = None

global debug
debug = False

global gridsize
gridsize = 5

# check for --width
if cmdline_args.world:
    print(f'using envs. width to {cmdline_args.world}')
    unity_world = f'envs/{cmdline_args.world}'

if cmdline_args.gridsize:
    print(f'using gridsize  {cmdline_args.gridsize}')
    gridsize = int(cmdline_args.gridsize)

if cmdline_args.debug:
    debug = True




print(f'Debug flag = {debug}')



print("Python version:")
print(sys.version)

# check Python version
if (sys.version_info[0] < 3):
    raise Exception("ERROR: ML-Agents Toolkit (v0.3 onwards) requires Python 3")

global environment
global this_session
global controller
global clock

class Component(ApplicationSession):
    """
    An application component that subscribes and receives events, and
    stop after having received 5 events.
    """

    async def onJoin(self, details):

        def on_event(msg, details=None, wtf=None):
            if debug:
                print("Got event {}".format(msg))

            step_world(msg)

        await self.subscribe(on_event, u'ai.leela.sms.render.json')
#                                  options=SubscribeOptions(details_arg='details'))

    def onDisconnect(self):
        asyncio.get_event_loop().stop()


# We will get the debuginfo (contains object locations) from dashboard message, it looks like
# [{'name': 'h', 'x': 2, 'y': 3, 'color': 'rgb(0,0,255)', 'shape': None, 'texture': '02', 'rotation': 0, 'grasping': True, 'graspedObject': None}, {'name': 'j', 'x': -100, 'y': -100, 'color': 'rgb(255,165,0)', 'shape': None, 'texture': '03', 'rotation': 0, 'grasping': False, 'graspedObject': None}, {'name': 'v', 'x': 1, 'y': 4, 'color': 'rgb(255,255,255)', 'shape': 'circle', 'texture': '00'}, {'name': 'b1', 'x': 1, 'y': 5, 'color': 'rgb(255,20,147)', 'shape': 'circle', 'texture': '00'}, {'name': 'b2', 'x': 5, 'y': 5, 'color': 'rgb(255,255,0)', 'shape': 'square', 'texture': '01'}]

def step_world(info):
    objlocs = info['debuginfo']
    locs = {'objs': objlocs} # make a dict with just one key 'objs' that maps to a list of obj descriptior dicts, 'objs' => [ {'name': 'b1', 'x': 1, 'y': 5}, ... ]
    #print('step_world action=', action)
    objlocs_string = json.dumps(locs) # convert back to serialized json string to pass into Unity as the 'action' string
    environment_state = environment.step(text_action=objlocs_string)['GridWorldLearning']

gridsize = 5


#================================================================


if __name__ == '__main__':
    import six

    if ( cmdline_args.interactive == True):
        environment = UnityEnvironment(file_name=None)
    else:
        environment = UnityEnvironment(file_name=unity_world)
    environment.reset(train_mode=False)


    url = environ.get("LEELA_AUTOBAHN_ROUTER", u"ws://127.0.0.1:1964/ws")
    if six.PY2 and type(url) == six.binary_type:
        url = url.decode('utf8')
    realm = u"leela"
    runner = ApplicationRunner(url, realm)
    runner.run(Component)


