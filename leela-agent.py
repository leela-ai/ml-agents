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
import json
from autobahn.asyncio.component import Component, run
from math import ceil

from autobahn.wamp.types import RegisterOptions

import asyncio
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

component = Component(
    transports=[
        {
            "type": "websocket",
            "url": u"wss://localhost:1964/ws",
            "endpoint": {
                "type": "tcp",
                "host": "localhost",
                "port": 1964,
                #                "tls": context,
            },
            "options": {
                "open_handshake_timeout": 100,
            }
        },
    ],
    realm=u"leela",
)


@component.on_join
def join(session, details):
    global this_session
    print("joined {}".format(details))
    this_session = session

@component.register(
    u"ai.leela.sms.step_world"
)
def step_world(actionsJS):
    actions = json.loads(actionsJS)

    if (len(actions) == 0):
        action = "nullaction"
    else:
        action = actions[0]

    #print('step_world action=', action)

    environment_state = environment.step(text_action=action)['GridWorldLearning']
    #print('environment_state = ',environment_state)
    text_obs = environment_state.text_observations[0]
    response = construct_response_with_environment_state(text_obs, [action])
    return (json.dumps(response))

# use environment.step(vector_action=None, text_action="handf")


@component.register(
    u"ai.leela.sms.do_something_in_sms"
)

# commands supported are:
# 'step_action'   {action: ACTION_NAME, param1: v1, param2: v2, ...}
#
def do_something_in_sms(commandJS, paramsJS):
    what = json.loads(commandJS)
    params = json.loads(paramsJS)
    if (what == "step_action"):
        action = params
        event = controller.step(action)
        return(json.dumps(event.metadata))
    else:
        print(f"do_something_in_sms: unsupported command {what}")
        return json.dumps(False)



def construct_field_sensors(sensors,minx,maxx,miny,maxy):
    for i in range  (minx, maxx + 1):
        for j in range (miny,maxy + 1):
                hpname = "hp%02d%02d" % (i,j)
                vfname = "vf%02d%02d" % (i, j)
                vpname = "vp%02d%02d" % (i, j)
                sensors[hpname] = True
                sensors[vfname] = True
                sensors[vpname] = True

def construct_foveal_shape_sensors (sensors,minx,maxx,miny,maxy):
    for i in range  (minx, maxx + 1):
        for j in range (miny,maxy + 1):
                fvsname1 = "fvs%d%d.circle" % (i, j)
                sensors[fvsname1] = True
                fvsname2 = "fvs%d%d.triangle" % (i, j)
                sensors[fvsname2] = True
                fvsname3 = "fvs%d%d.square" % (i, j)
                sensors[fvsname3] = True

def construct_foveal_sensors (sensors,minx,maxx,miny,maxy):
    for i in range  (minx, maxx + 1):
        for j in range (miny,maxy + 1):
            for k in range (0,16):
                fvname1 = "fov%d%d.%d" % (i,j,k)
                sensors[fvname1] = True


def construct_sensors(minx,maxx,miny,maxy):
    sensors = {}
    construct_field_sensors(sensors,minx,maxx,miny,maxy)
    construct_foveal_shape_sensors(sensors,1,3,1,3)
    construct_foveal_sensors(sensors,1,3,1,3)
    return sensors

gridsize = 5


def usableActionNames():
    return ["nullaction","handl","handr","handf","handb","grasp","ungrasp","eyel","eyer","eyef","eyeb"]

@component.register(
    u"ai.leela.sms.get_capabilities"
)

def get_capabilities():
    c = {}
    c["sensors"] = {}
    sensors = construct_sensors(1,gridsize,1,gridsize)
    c["sensors"]["items"] =  sensors
    c["sensors"]["actions"] = usableActionNames()
    return json.dumps(c)

#def get_capabilities():
#    return capabilities_string


# observation_vector is an encoded string that has sensor names mapped to boolean values,  name1=val1;name2=val2;...
# e.g., "hp11=0;hp12=1;hp34=0;tactr=1;..."
#
# return   a dict of {itemName1: v1, itemName2, v2, ...}
def decode_text_observation_string(item_string):
    #print('item_string=', item_string)
    items = item_string[0:-1] # trim off trailing ';'
    kv = items.split(";")
    # splits into ['hp11=0', 'hp12=1', 'hp34=0', 'tactr=1']

    kvp = map(lambda pair: pair.split('='), kv)
    # now kvp = [['hp11', '0'], ['hp12', '1'], ['hp34', '0'], ['tactr', '1']]

    # make a dictionary from this list, convert 0 and 1 into True and False for json encoding
    vals = {}
    for pair in kvp:
        itemname = pair[0]
        itemval = pair[1]
        if (itemval == "1"):
            json_bool_val = True
        else:
            json_bool_val = False
            
        vals[itemname] = json_bool_val

    return vals

def construct_response_with_environment_state(observation_vector,action):
   global this_session
   response = {}
   #items is a dict of {itemName1: v1, itemName2, v2, ...}
   items = decode_text_observation_string(observation_vector);
   response['items'] = items
   response['actions'] = action
   # fixme : debuginfo wants a list of all object positions
   #processed_objects = list(map(lambda obj: {'name':obj.name,'x': int(obj.pos.x),'y':int(obj.pos.y)},objects))
   #response['debuginfo'] =  processed_objects
   response['debuginfo'] =  []
   this_session.publish("ai.leela.sms.render.json", response)
   if debug:
       print("Response",response)
   return response

if __name__ == "__main__":
    if ( cmdline_args.interactive == True):
        environment = UnityEnvironment(file_name=None)
    else:
        environment = UnityEnvironment(file_name=unity_world)
    environment.reset(train_mode=False)
    run([component])

