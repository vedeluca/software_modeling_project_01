﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonProcessing
{
    internal class JsonArray<T> : List<T>, JsonNode
    {
        public JsonNode? Parent { get; set; }
        public JsonNode? Root { get; set; }

        public JsonArray() { }

        public JsonArray(JsonNode parent)
        {
            Parent = parent;
            if (parent.Root != null)
                Root = parent.Root;
            else
                Root = parent;
        }

        new public string ToString()
        {
            return this.ToString("");
        }

        public string ToString(string tabs)
        {
            StringBuilder sb = new();
            sb.Append("[\n");
            for (int i = 0; i < this.Count; i++)
            {
                sb.Append(tabs);
                sb.Append('\t');
                T item = this[i];
                if (item == null)
                {
                    sb.Append("null");
                }
                else if (item is string) {
                    sb.Append('"');
                    sb.Append(item);
                    sb.Append('"');
                }
                else if (item is JsonNode)
                {
                    StringBuilder tabsBuilder = new();
                    tabsBuilder.Append(tabs);
                    tabsBuilder.Append('\t');
                    JsonNode node = (JsonNode)item;
                    sb.Append(node.ToString(tabsBuilder.ToString()));
                }
                else
                {
                    sb.Append(item.ToString());
                }
                if (i < this.Count - 1)
                    sb.Append(',');
                sb.Append('\n');
            }
            sb.Append(tabs);
            sb.Append(']');
            return sb.ToString();
        }

        public string QueryToString(string search)
        {
            object? query = Query(search);
            if (query == null)
                return "null";
            else if (query is JsonNode)
                return ((JsonNode)query).ToString();
            else
                return query.ToString();
        }

        public object Query(string search)
        {
            Type genericType = typeof(T);
            Type interfaceType = typeof(JsonNode);
            if (interfaceType.IsAssignableFrom(genericType))
            {
                foreach (JsonNode? node in this)
                {
                    if (null != node)
                    {
                        object queryResult = node.Query(search);
                        if (queryResult != null)
                            return queryResult;
                    }
                }
            }
            return null;
        }
    }
}