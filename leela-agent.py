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
# inverse of THOR's idea of grid, this means 4 grids per unit of space, i.e., a Thor gridsize=0.25


RANDOMIZE_ITEMS_PERIOD = 100

GRID_MINX = 1
GRID_MAXX = 9
GRID_MINY = 1
GRID_MAXY = 9

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


NULLACTION = dict(action='Contains', objectId='foo')


@component.register(
    u"ai.leela.sms.step_world"
)
def step_world(actionsJS):
   actions = json.loads(actionsJS)
   if(len(actions) == 0):
      integer_action = 0
   else:
      action = dict(action=actions[0])
      action = action['action']
      if debug:
          print("Action:", action)
      if(action == "NoAction"):
        integer_action = 0
      elif(action == "Right"):
        integer_action = 1
      elif(action == "Left"):
        integer_action = 2
      elif(action == "Up"):
        integer_action = 3
      elif(action == "Down"):
        integer_action = 4

   environment_state =environment.step([integer_action])['GridWorldLearning'].vector_observations
   response = construct_response_with_environment_state(environment_state[0])
   return(json.dumps(response))

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
    # send back a list of all primtive item names
    for name in allItemNames():
        items[name] = True
    actions_list = [
        "NoAction","Left", "Right", "Up", "Down",
    ]
    sensors['actions'] = actions_list
    sensors['items'] = items
    print(wrapper)
    return (json.dumps(wrapper))



# we need to return a list of primitive item names when we respond to get_capabilities
def allItemNames():
    itemNames = []
    # absolute body position names
    for x in range(GRID_MINX, GRID_MAXX):
        for y in range(GRID_MINY, GRID_MAXY):
            locname = f'hp{x:d}{y:d}'
            itemNames.append(locname)
    return itemNames

# Adds agent position items to the ITEMS dict
def map_agent_position_to_grid_sensor_items(items, observation_vector):
   clear_position_items(items)
   if debug:
       print("Observation vector",observation_vector)
   agent_x = observation_vector[0]
   agent_z = observation_vector[1]
   grid_x = 1+int(ceil(agent_x))
   grid_z = 1+int(ceil(agent_z))
   locname = f'hp{grid_x:d}{grid_z:d}'
   if debug:
       print('locname',locname)
   items[locname] = True

def clear_position_items(items):
    for x in range(GRID_MINX, GRID_MAXX):
        for y in range(GRID_MINY, GRID_MAXY):
            locname = f'hp{x:d}{y:d}'
            items[locname] = False
def map_observation_vector_action_to_text(action):
    if (action == 0):
        return_action = "NoAction"
    elif (action == 1):
        return_action = "Right"
    elif (action == 2):
        return_action = "Left"
    elif (action == 3):
        return_action = "Up"
    elif (action == 4):
        return_action = "Down"
    return([return_action])

def construct_response_with_environment_state(observation_vector):
   global this_session
   response = {}
   #items is a dict of {itemName1: v1, itemName2, v2, ...}
   items = {}
   map_agent_position_to_grid_sensor_items(items,observation_vector)
   response['items'] = items
   response['actions'] = map_observation_vector_action_to_text(observation_vector[6])
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
