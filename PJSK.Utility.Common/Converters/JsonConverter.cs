using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJSK.Utility.Common.Converters
{
    public static class JsonConverter
    {
        public static TConvertTo ConvertType<TConvertTo>(object objConvertFrom)
        {
            return JsonConvert.DeserializeObject<TConvertTo>(JsonConvert.SerializeObject(objConvertFrom));
        }
    }
}
