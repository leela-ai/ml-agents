using System;
using System.Collections.Generic;

namespace Blocksworld
{
    public class SensorState
    {
        public Dictionary<String, SensorInput> items = new Dictionary<String, SensorInput>();
        public List<String> actions = new List<String>();
        public List<Dictionary<String,Object>> debugInfo = new List<Dictionary<String,Object>>();
        public long clock = 0;

        public SensorState()
        {
        }

        public Dictionary<String,SensorInput> Items()
        {
            return items;
        }

        public void setClock(long c)
        {
            clock = c;
        }
        public long getClock()
        {
            return clock;
        }

        public void setActions(List<String> a)
        {
            actions = a;
        }

        public void setDebugInfo(List<Dictionary<String,Object>> d)
        {
            debugInfo = d;
        }

        public bool getSensorValue(String name)
        {
            SensorInput s = items[name];
            if (s == null)
            {
                s = new SensorInput(name, false);
            }
            return s.value;
        }

        // Sensor items are only distinguished as being proprioceptive or visual by the name

        public SensorInput getSensorItem(String name)
        {
            SensorInput s = items[name];
            if (s == null)
            {
                s = new SensorInput(name, false);
            }
            return s;
        }

        /**
           Item list, sorted by item name
         */
        public List<SensorInput> sortedItems()
        {
            /*
            ArrayList<SensorInput> sorted = new ArrayList<>();
            for (Map.Entry<String, SensorInput> entry : items.entrySet())
            {
                sorted.add(entry.getValue());
            }

            Collections.sort(sorted,
                    ((s1, s2)->s1.name.compareTo(s2.name)));


            return sorted;
            */
            return null; // TODO Not Yet Implemented

        }

        public SensorInput setSensorValue(String name, bool val, long clock)
        {
            SensorInput s = items[name];
            if (s == null)
            {
                s = new SensorInput(name, val);
                items.Add(name, s);
            }
            else
            {
                s.setValue(val, clock);
            }
            return s;
        }

        public String toString()
        {
            /**
             * StringBuilder out = new StringBuilder();
            ArrayList<String> list = new ArrayList<String>();
            for (Map.Entry<String, SensorInput> entry : items.entrySet())
            {
                list.add(entry.getValue().toString());
            }
            Collections.sort(list);
            for (String s: list)
            {
            out.append(s + "\n");
            }

            return out.toString();
    */
            return "TODO:SensorState.ToString Not Yet Implemented"
        }


        public Dictionary<String,Object> toMap()
        {
            Dictionary<String, Object> obj = new Dictionary<string, object>();
            Dictionary<String,bool> itemSet = new Dictionary<string, bool>();

            foreach (KeyValuePair<String,SensorInput> entry in items)
            {
                SensorInput item = entry.Value;
                itemSet.put(entry.Key, item.value);
            }
            obj.Add("items", itemSet);
            obj.Add("actions", actions);
            obj.Add("clock", clock);
            obj.Add("debuginfo", debugInfo);
            return obj;
        }


        public String toJSONString()
        {
            /*
            Map obj = new LinkedHashMap();
            Map itemSet = new LinkedHashMap();
            for (Map.Entry<String, SensorInput> entry : items.entrySet())
            {
                SensorInput item = entry.getValue();
                itemSet.put(entry.getKey(), item.value);
            }
            obj.put("items", itemSet);
            obj.put("actions", actions);
            obj.put("clock", clock);
            return JSONValue.toJSONString(obj);
            */
            // TODO
            return "{\"sensorstate-to-json-nyi\": true}";
        }


    }
}
