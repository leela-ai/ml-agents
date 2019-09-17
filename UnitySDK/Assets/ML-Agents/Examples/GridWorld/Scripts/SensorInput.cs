using System;
using System.Collections.Generic;

namespace Blocksworld
{
   
    public class SensorInput  {
    public string name;
    public bool value;
    public long clock;

    public SensorInput(String name, bool val)
    {
        this.name = name;
        this.value = val;
    }

    public void setValue(bool val, long time)
    {
        this.value = val;
        this.clock = time;
    }

    public String toString()
    {
        return $"sensorinput-{name}: {value}";
    }

    
        // for serializing to send to browser for visualizing
        public String toJSONString()
    {
        Dictionary<String,object> obj = new Dictionary<string, object>();
        obj.Add("name", name);
        obj.Add("state", value);
        obj.Add("clock", clock);

        return "{\"JSON_not_yet_implemented\": true}";

         
    }

}


}