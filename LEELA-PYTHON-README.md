# Running Unity ML-Agents with Python "communicator" wrapper / wamp bridge



## Github project: leela-ai/ml-agents

There is a fork of the Unity ML-Agents project on github, at https://github.com/leela-ai/ml-agents

I am running Unity v2019.3.0a8 on OSX

ml-agents directory contains a subdir called UnitySDK which is where the Unity project lives. We have modified one of the example games, called GridWorld, to match what Leela's Java grid-world SMS does. 

There's python code in the ml-agents directory which talks to the Unity/C# ML-Agents framework via sockets with some protocol we don't know about. 

## PyCharm Project / create vent with Python 3.6

We've been using PyCharm, a python IDE from JetBrains, to debug and run python with. It's nice because it has a GUI to set up a python virtualenv, and install any needed libraries.

The .idea directory has the project all set up, but the vent directory is not checked in, so you need to create it. 

You need Python 3.6 installed, and "pip install ml-agents". This can be done from PyCharm ->  Preferences -> Project Interpreter menu: it will create a virtual env automatically, and you can then select ml-agents package to install. 

![pycharm-preferences](/Users/hminsky/PycharmProjects/ml-agents/doc-images/pycharm-preferences.png)

See [docs/Installation.md](docs/Installation.md) 



## Building a standalone GridWorld app in Unity



![unity-build](/Users/hminsky/PycharmProjects/ml-agents/doc-images/unity-build.png)



![build-save](/Users/hminsky/PycharmProjects/ml-agents/doc-images/build-save.png)

## /leela-agent.py WAMP bridge

The file /leela-agent.py in the top level dir is code which bridges from Leela WAMP SMS protocol to the ml-agents 'communicator' that talks to Unity. 

To run it, activate the venv

```bash
. venv/bin/activate
```



Then run 

`python leela-agent.py -I --gridsize 11`

This will run in 'interactive mode' which means it will wait for you to press "Start" in a running Unity IDE to fire up the Unity GridWorld app

or, if you've built a precompiled Unity GridWorld app, and put it into envs/GridWorld-11x11.app

`python leela-agent.py -W GridWorld-11x11 --gridsize 11`



