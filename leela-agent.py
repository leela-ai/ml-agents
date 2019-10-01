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

parser.add_argument("-W", "--world", help="Choose a prebuilt Unity world to run from envs/ directory",)
parser.add_argument("-d", "--debug", help="Enable debug printing", action="store_true")
parser.add_argument("-I", "--interactive", help="Run Unity game interactively", action="store_true")

cmdline_args = parser.parse_args()

# default to interactive launch of game script from Unity
unity_world = None

global debug
debug = False

# check for --width
if cmdline_args.world:
    print(f'using envs. width to {cmdline_args.world}')
    unity_world = f'envs/{cmdline_args.world}'

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

gridsize = 12


def usableActionNames():
    return ["nullaction","handl","handr","handf","handb","grasp","ungrasp","eyel","eyer","eyef","eyeb"]

@component.register(
    u"ai.leela.sms.get_capabilities"
)

def get_capabilities():
    c = {}
    c["sensors"] = {}
    sensors = construct_sensors(0,gridsize,0,gridsize)
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

capabilities_string = """
{"sensors":{"actions":["nullaction","handl","handr","handf","handb","grasp","ungrasp","eyel","eyer","eyef","eyeb"],"items":{"fvs11.square":true,"fov33.15":true,"fov33.14":true,"fov33.11":true,"fov33.10":true,"fov33.13":true,"fov33.12":true,"fvs13.square":true,"vp0002":true,"vp0003":true,"vp0004":true,"j.hgr":true,"vp0000":true,"vp0001":true,"jp0503":true,"jp0504":true,"jp0501":true,"jp0502":true,"jp0505":true,"fov21.15":true,"fov21.13":true,"fov21.14":true,"fov21.11":true,"fov21.12":true,"fvs32.triangle":true,"fov21.10":true,"fov11.00":true,"fov11.01":true,"fov11.02":true,"fov11.03":true,"fov11.04":true,"fov11.05":true,"fvs22.triangle":true,"fov11.06":true,"fov11.07":true,"fov11.08":true,"fov11.09":true,"fov22.15":true,"fov22.14":true,"fov22.13":true,"fov22.12":true,"fov22.11":true,"fov22.10":true,"fov33.08":true,"fvs11.triangle":true,"fov33.07":true,"fov33.09":true,"fov33.04":true,"hp0503":true,"fov33.03":true,"hp0504":true,"jp0401":true,"fov33.06":true,"hp0505":true,"fov33.05":true,"fov33.00":true,"jp0404":true,"jp0405":true,"fov33.02":true,"jp0402":true,"fov33.01":true,"fvs23.circle":true,"jp0403":true,"fov22.09":true,"fvs12.circle":true,"vf0505":true,"fov22.08":true,"hp0501":true,"vf0504":true,"fov22.07":true,"hp0502":true,"vf0503":true,"fov22.06":true,"vf0502":true,"fov22.05":true,"vf0501":true,"fov22.04":true,"fov22.03":true,"fov22.02":true,"fov22.01":true,"fov22.00":true,"fov32.07":true,"fov32.06":true,"fov32.09":true,"fov32.08":true,"fov32.03":true,"fvs21.triangle":true,"fov32.02":true,"fov32.05":true,"fov32.04":true,"fov32.01":true,"fov32.00":true,"fvs33.square":true,"fvs31.square":true,"vp0303":true,"vp0304":true,"vp0200":true,"j.hcl":true,"vp0201":true,"vp0202":true,"vp0203":true,"fov32.14":true,"jp0301":true,"fov32.13":true,"jp0302":true,"fov32.15":true,"fov32.10":true,"jp0305":true,"fov32.12":true,"jp0303":true,"fov32.11":true,"jp0304":true,"fvs11.circle":true,"vf0405":true,"vf0404":true,"vf0403":true,"vf0402":true,"vf0401":true,"fov31.13":true,"fov31.12":true,"fov31.15":true,"fov31.14":true,"fov31.11":true,"fov31.10":true,"fov21.08":true,"fov21.09":true,"fvs31.triangle":true,"fov21.06":true,"fov21.07":true,"fov21.04":true,"fov21.05":true,"fov21.02":true,"fov21.03":true,"fvs22.square":true,"fov21.00":true,"vp0204":true,"fov21.01":true,"vp0101":true,"vp0102":true,"vp0103":true,"vp0104":true,"vp0100":true,"jp0202":true,"jp0203":true,"jp0201":true,"jp0204":true,"jp0205":true,"fvs12.triangle":true,"vf0305":true,"vf0304":true,"vf0303":true,"vf0302":true,"vf0301":true,"fvs33.triangle":true,"h.text0":true,"h.text2":true,"h.text1":true,"fvs12.square":true,"fov13.11":true,"fov13.10":true,"fov13.02":true,"fov13.01":true,"fov13.04":true,"fov13.03":true,"fov13.06":true,"fov13.05":true,"h.tactf":true,"fov13.08":true,"fov13.07":true,"fov13.09":true,"h.tactr":true,"h.hgr":true,"h.tactl":true,"fov31.09":true,"fvs33.circle":true,"vp0400":true,"vp0401":true,"fov31.06":true,"fov31.05":true,"fov31.08":true,"fov31.07":true,"fov31.02":true,"fvs22.circle":true,"jp0103":true,"fov31.01":true,"jp0104":true,"fov31.04":true,"fvs31.circle":true,"jp0101":true,"fov31.03":true,"jp0102":true,"fov13.00":true,"fov31.00":true,"jp0105":true,"hp0201":true,"hp0202":true,"hp0203":true,"hp0204":true,"hp0205":true,"vf0205":true,"h.text3":true,"vf0204":true,"vf0203":true,"vf0202":true,"vf0201":true,"fvs23.square":true,"fvs13.triangle":true,"vp0402":true,"vp0403":true,"vp0404":true,"vp0300":true,"vp0301":true,"vp0302":true,"fov13.13":true,"fov13.12":true,"fov13.15":true,"hp0101":true,"fov13.14":true,"hp0102":true,"hp0103":true,"hp0104":true,"hp0105":true,"vf0105":true,"vf0104":true,"vf0103":true,"vf0102":true,"vf0101":true,"fov23.10":true,"fov23.09":true,"fov23.08":true,"fov23.07":true,"fov23.06":true,"fov23.05":true,"fov23.04":true,"h.hcl":true,"fov23.03":true,"fov23.02":true,"fov23.01":true,"fvs32.square":true,"fov23.00":true,"hp0404":true,"fvs13.circle":true,"hp0405":true,"fov11.10":true,"fov11.11":true,"fov11.12":true,"fov11.13":true,"fov11.14":true,"fov11.15":true,"hp0401":true,"hp0402":true,"hp0403":true,"fov12.12":true,"fov12.11":true,"fov12.14":true,"fov12.13":true,"fov12.15":true,"fvs21.square":true,"fvs32.circle":true,"hp0305":true,"h.tactb":true,"fov12.10":true,"fvs21.circle":true,"fov12.01":true,"fov12.00":true,"fov12.03":true,"fov12.02":true,"fov12.05":true,"hp0301":true,"fov12.04":true,"hp0302":true,"fov12.07":true,"hp0303":true,"fov12.06":true,"hp0304":true,"fov12.09":true,"fov12.08":true,"fov23.15":true,"fov23.14":true,"fov23.13":true,"fvs23.triangle":true,"fov23.12":true,"fov23.11":true}}}
"""




if __name__ == "__main__":
    if ( cmdline_args.interactive == True):
        environment = UnityEnvironment(file_name=None)
    else:
        environment = UnityEnvironment(file_name=unity_world)
    environment.reset(train_mode=False)
    run([component])

