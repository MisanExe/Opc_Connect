using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeSysAppNs;
using Opc.UaFx;

namespace CodeSysAppNs
{
    public partial class CodeSysApp
    {
        //Write

        /// <summary>
        ///  Write tag for boolean 
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="Value"></param>
        public void WriteTag(string TagName, bool Value)
        {
            if (GetTagByName(TagName) != null)
            {
                this.client?.WriteNode(GetTagByName(TagName)?.NodeId, Value);
            }

        }

        /// <summary>
        ///  Write tag for int
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="Value"></param>
        public void WriteTag(string TagName, int Value)
        {
            if (GetTagByName(TagName) != null)
            {
                this.client?.WriteNode(GetTagByName(TagName)?.NodeId, Value);
            }

        }

        /// <summary>
        /// Write tag for string
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="Value"></param>
        public void WriteTag(string TagName, string Value)
        {
            if (GetTagByName(TagName) != null)
            {
                this.client?.WriteNode(GetTagByName(TagName)?.NodeId, Value);
            }

        }

        /// <summary>
        /// Write tage for double
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="Value"></param>
        public void WriteTag(string TagName, double Value)
        {
            if (GetTagByName(TagName) != null)
            {
                this.client?.WriteNode(GetTagByName(TagName)?.NodeId, Value);
            }

        }


        //Read
        /// <summary>
        /// Reads tag. Returns { True, False, null}
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        public OpcValue? ReadTag(string ? TagName)
        {
            return this?.client?.ReadNode(GetTagByName(TagName)?.NodeId);   
        }

        public  Object ConvertOpcValue(OpcValue ?value)
        {
            Object? ret = null;
            var tempVal = 0;

            if (value.Value is bool)
            {
                ret = (bool)value.Value;
                return ret;

            }
            else if (value.Value is string)
            {
                ret =  (string)value.Value;
                return ret;

            }
            else if (value.Value is decimal)
            {
                ret = (int)value.Value;
                return ret;
            }
            else if (int.TryParse(value.Value.ToString(), out tempVal))
            {
                ret =  tempVal;
                return ret;
            }

            return ret;
        }



    }
}
