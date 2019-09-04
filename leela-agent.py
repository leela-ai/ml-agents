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
import argparse

# initiate the parser
parser = argparse.ArgumentParser(description='Leela Crossbar agent for Unity ML Agents worlds')

parser.add_argument("-W", "--world", help="Choose a prebuilt Unity world to run from envs/ directory",)
parser.add_argument("-d", "--debug", help="Enable debug printing", action="store_true")

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

import json
from autobahn.asyncio.component import Component, run
from math import ceil

from autobahn.wamp.types import RegisterOptions

import asyncio
import ssl

import matplotlib.pyplot as plt
import numpy as np
import sys

from mlagents.envs import UnityEnvironment

print("Python version:")
print(sys.version)

# check Python version
if (sys.version_info[0] < 3):
    raise Exception("ERROR: ML-Agents Toolkit (v0.3 onwards) requires Python 3")



global environment
global this_session
global controller
global clock

PROP_GRID_MINX = 1
PROP_GRID_MAXX = 9
PROP_GRID_MINY = 1
PROP_GRID_MAXY = 9

VISUAL_GRID_MINX = 1
VISUAL_GRID_MAXX = 5
VISUAL_GRID_MINY = 1
VISUAL_GRID_MAXY = 5

EYE_LEFT_LIMIT = 0
EYE_RIGHT_LIMIT = 4
EYE_UP_LIMIT = 4
EYE_DOWN_LIMIT = 0

AGENT_X_POS_LOC_IN_OBS_VEC = 0
AGENT_Y_POS_LOC_IN_OBS_VEC = 1
GOAL_X_POS_LOC_IN_OBS_VEC = 2
GOAL_Y_POS_LOC_IN_OBS_VEC = 3
PIT_X_POS_LOC_IN_OBS_VEC = 4
PIT_Y_POS_LOC_IN_OBS_VEC = 5
ACTION_LOC_IN_OBS_VEC = 6

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
    global eye_pos
    print("joined {}".format(details))
    this_session = session
    eye_pos = eyePos()

@component.register(
    u"ai.leela.sms.step_world"
)
def step_world(actionsJS):
    actions = json.loads(actionsJS)

    if (len(actions) == 0):
        action = "NoAction"
        integer_action = 0
    else:
        action = dict(action=actions[0])
        action = action['action']
        integer_action = check_action(action)

    if (integer_action == -1):
        handle_eye_action_in_python(action)
        environment_state = environment.step([0])['GridWorldLearning'].vector_observations
    else:
        environment_state = environment.step([integer_action])['GridWorldLearning'].vector_observations
    response = construct_response_with_environment_state(environment_state[0], [action],eye_pos.x, eye_pos.y)
    return (json.dumps(response))


def check_action(action):
    integer_action = -1

    if (action == "NoAction"):
       integer_action = 0
    elif (action == "Right"):
       integer_action = 1
    elif (action == "Left"):
       integer_action = 2
    elif (action == "Up"):
       integer_action = 3
    elif (action == "Down"):
       integer_action = 4
    elif (action == "Grasp"):
       integer_action = 5
    elif (action == "Ungrasp"):
       integer_action = 6

    return integer_action

def limit_move_eye_left():
    if (eye_pos.x != EYE_LEFT_LIMIT):
        eye_pos.x -= 1

def limit_move_eye_right():
    if (eye_pos.x != EYE_RIGHT_LIMIT):
        eye_pos.x += 1
def limit_move_eye_up():
    if (eye_pos.y != EYE_UP_LIMIT):
        eye_pos.y += 1

def limit_move_eye_down():
    if (eye_pos.y != EYE_DOWN_LIMIT):
        eye_pos.y -= 1

def handle_eye_action_in_python(action):
    if (action == "EyeLeft"):
        limit_move_eye_left()
    elif (action == "EyeRight"):
        limit_move_eye_right()
    elif (action == "EyeUp"):
        limit_move_eye_up()
    elif (action == "EyeDown"):
        limit_move_eye_down()



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


@component.register(
    u"ai.leela.sms.get_capabilities"
)
def get_capabilities():
    wrapper = {}
    sensors = {}
    wrapper['sensors'] = sensors
    items = {}
    # send back a list of all primitive item names
    for name in allItemNames():
        items[name] = True
    actions_list = [
        "NoAction","Left", "Right", "Up", "Down","EyeLeft", "EyeRight","EyeUp","EyeDown","Grasp","Ungrasp"
    ]
    sensors['actions'] = actions_list
    sensors['items'] = items
    print(wrapper)
    return (json.dumps(wrapper))


class eyePos():

    def __init__(self,x: int=2,y: int=2,):
        self.x = x
        self.y = y



# we need to return a list of primitive item names when we respond to get_capabilities
def allItemNames():
    itemNames = []
    # absolute body position names
    for x in range(PROP_GRID_MINX, PROP_GRID_MAXX):
        for y in range(PROP_GRID_MINY, PROP_GRID_MAXY):
            proprioceptive_grid_locname = f'hp{x:d}{y:d}'
            itemNames.append(proprioceptive_grid_locname)

    for x in range(VISUAL_GRID_MINX, VISUAL_GRID_MAXX):
        for y in range(VISUAL_GRID_MINY, VISUAL_GRID_MAXY):
            visual_grid_locname = f'vf{x:d}{y:d}'
            itemNames.append(visual_grid_locname)
    return itemNames

# Adds agent position items to the ITEMS dict
def update_proprioceptive_grid_sensor_items(items, observation_vector,x_pos_loc,y_pos_loc):
   if debug:
       print("Observation vector",observation_vector)
   agent_x = observation_vector[x_pos_loc]
   agent_z = observation_vector[y_pos_loc]
   grid_x = 1+int(ceil(agent_x))
   grid_z = 1+int(ceil(agent_z))
   locname = f'hp{grid_x:d}{grid_z:d}'
   if debug:
       print('proprioceptive grid locname',locname)
   items[locname] = True



def update_visual_grid_sensor_items(items, observation_vector,x_pos_loc,y_pos_loc,eye_pos_x,eye_pos_y):
   if debug:
       print("Observation vector",observation_vector)
   agent_x = observation_vector[x_pos_loc]
   agent_z = observation_vector[y_pos_loc]
   grid_x = (1+int(ceil(agent_x)) - eye_pos_x)
   grid_z = (1+int(ceil(agent_z)) - eye_pos_y)
   locname = f'vf{grid_x:d}{grid_z:d}'
   if debug:
       print('visual grid locname', locname)
   items[locname] = True




def clear_proprioceptive_grid_items(items):
    for x in range(PROP_GRID_MINX, PROP_GRID_MAXX):
        for y in range(PROP_GRID_MINY, PROP_GRID_MAXY):
            locname = f'hp{x:d}{y:d}'
            items[locname] = False

def clear_visual_grid_items(items):
    for x in range(VISUAL_GRID_MINX, VISUAL_GRID_MAXX):
        for y in range(VISUAL_GRID_MINY, VISUAL_GRID_MAXY):
            locname = f'vf{x:d}{y:d}'
            items[locname] = False



def construct_response_with_environment_state(observation_vector,action,eye_pos_x,eye_pos_y):
   global this_session
   response = {}
   #items is a dict of {itemName1: v1, itemName2, v2, ...}
   items = {}
   clear_proprioceptive_grid_items(items)
   update_proprioceptive_grid_sensor_items(items,observation_vector,AGENT_X_POS_LOC_IN_OBS_VEC,AGENT_Y_POS_LOC_IN_OBS_VEC)
   update_proprioceptive_grid_sensor_items(items,observation_vector,GOAL_X_POS_LOC_IN_OBS_VEC,GOAL_Y_POS_LOC_IN_OBS_VEC)
   update_proprioceptive_grid_sensor_items(items,observation_vector,PIT_X_POS_LOC_IN_OBS_VEC,PIT_Y_POS_LOC_IN_OBS_VEC)
   clear_visual_grid_items(items)
   update_visual_grid_sensor_items(items, observation_vector, AGENT_X_POS_LOC_IN_OBS_VEC,AGENT_Y_POS_LOC_IN_OBS_VEC,eye_pos_x,eye_pos_y)
   update_visual_grid_sensor_items(items, observation_vector, GOAL_X_POS_LOC_IN_OBS_VEC, GOAL_Y_POS_LOC_IN_OBS_VEC,eye_pos_x,eye_pos_y)
   update_visual_grid_sensor_items(items, observation_vector, PIT_X_POS_LOC_IN_OBS_VEC,PIT_Y_POS_LOC_IN_OBS_VEC,eye_pos_x,eye_pos_y)
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
   #environment = UnityEnvironment(file_name=None)

   environment = UnityEnvironment(file_name=unity_world)
   environment.reset(train_mode=False)
   run([component])
