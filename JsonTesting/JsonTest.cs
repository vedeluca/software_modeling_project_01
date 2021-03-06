using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonProcessing;

namespace JsonTesting
{
    [TestClass]
    public class JsonTest
    {
        private string JsonString;
        private string BrokenString;

        [TestInitialize]
        public void JsonTestInit()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");//1
            sb.Append("\t\"glossary\": {\n");//2
            sb.Append("\t\t\"title\": \"example glossary\",\n");//3
            sb.Append("\t\t\"GlossDiv\": {\n");//4
            sb.Append("\t\t\t\"title\": \"S\",\n");//5
            sb.Append("\t\t\t\"GlossList\": {\n");//6
            sb.Append("\t\t\t\t\"GlossEntry\": {\n");//7
            sb.Append("\t\t\t\t\t\"ID\": 123,\n");//8
            sb.Append("\t\t\t\t\t\"SortAs\": true,\n");//9
            sb.Append("\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\n");//10
            sb.Append("\t\t\t\t\t\"Acronym\": -4.56,\n");//11
            sb.Append("\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\n");//12
            sb.Append("\t\t\t\t\t\"GlossDef\": {\n");//13
            sb.Append("\t\t\t\t\t\t\"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\n");//14
            sb.Append("\t\t\t\t\t\t\"GlossSeeAlso\": [\n");//15
            sb.Append("\t\t\t\t\t\t\t\"GML\",\n");//16
            sb.Append("\t\t\t\t\t\t\t\"XML\"\n");//17
            sb.Append("\t\t\t\t\t\t]\n");//18
            sb.Append("\t\t\t\t\t},\n");//16
            sb.Append("\t\t\t\t\t\"GlossSee\": null\n");//17
            sb.Append("\t\t\t\t}\n");//18
            sb.Append("\t\t\t}\n");//19
            sb.Append("\t\t}\n");//20
            sb.Append("\t}\n");//21
            sb.Append("}");//22
            JsonString = sb.ToString();

            StringBuilder broken = new StringBuilder();
            broken.Append("{\n");//1
            broken.Append("\t\"glossary\": {\n");//2
            broken.Append("\t\t\"title\": \"example glossary\",\n");//3
            broken.Append("\t\t\"GlossDiv\": {\n");//4
            broken.Append("\t\t\t\"title\": \"S\",\n");//5
            broken.Append("\t\t\t\"GlossList\": {\n");//6
            broken.Append("\t\t\t\t\"GlossEntry\": {\n");//7
            broken.Append("\t\t\t\t\t\"ID\": 123,\n");//8
            broken.Append("\t\t\t\t\t\"SortAs\": true,\n");//9
            broken.Append("\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\n");//10
            broken.Append("\t\t\t\t\t\"Acronym\": -4.56,\n");//11
            broken.Append("\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\n");//12
            broken.Append("\t\t\t\t\t\"GlossDef\": {\n");//13
            broken.Append("\t\t\t\t\t\t\"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\n");//14
            broken.Append("\t\t\t\t\t\t\"GlossSeeAlso\": [\n");//15
            broken.Append("\t\t\t\t\t\t\t\"GML\",\n");//16
            broken.Append("\t\t\t\t\t\t\t\"XML\"\n");//17
            broken.Append("\t\t\t\t\t\t}\n");//18............This is where it should break
            broken.Append("\t\t\t\t\t},\n");//16
            broken.Append("\t\t\t\t\t\"GlossSee\": null\n");//17
            broken.Append("\t\t\t\t}\n");//18
            broken.Append("\t\t\t}\n");//19
            broken.Append("\t\t}\n");//20
            broken.Append("\t}\n");//21
            broken.Append("}");//22
            BrokenString = broken.ToString();
        }

        [TestMethod]
        public void TransformStringToObject()
        {
            IJsonNode? root = JsonParser.StringToJsonNode(JsonString);
            Assert.IsNotNull(root, "Root node is null");
            string json = root.ToString();
            Assert.AreEqual(json, JsonString, "Output string does not match input string");
            Console.WriteLine(json);
        }

        [TestMethod]
        public void ThrowJsonParserException()
        {
            Assert.ThrowsException<JsonParserException>(() => JsonParser.StringToJsonNode(BrokenString), "Exception expected for broken JSON string");
            try
            {
                JsonParser.StringToJsonNode(BrokenString);
            }
            catch (JsonParserException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void AddJsonObject()
        {
            IJsonNode? root = JsonParser.StringToJsonNode(JsonString);
            Assert.IsNotNull(root, "Root node is null");
            Assert.IsInstanceOfType(root, typeof(JsonObject<string, object?>), "Root node is not an object");
            JsonObject<string, object?> rootObj = (JsonObject<string, object?>)root;
            JsonObject<string, object?> node = new(rootObj);
            node.Add("Boolean Test", false);
            node.Add("Number Test", -123.45);
            node.Add("String Test", "test \\\"test\\\"");
            rootObj.Add("node", node);
            object? val = rootObj["node"];
            Assert.IsNotNull(val, "Value in root is null");
            Assert.IsInstanceOfType(val, typeof(JsonObject<string, object?>), "Value in root is not JsonObject type");
            JsonObject<string, object?> valObj = (JsonObject<string, object?>)val;
            Assert.AreEqual(valObj["Boolean Test"], node["Boolean Test"], "Objects do not have matching values");
            Assert.AreEqual(valObj["Number Test"], node["Number Test"], "Objects do not have matching values");
            Assert.AreEqual(valObj["String Test"], node["String Test"], "Objects do not have matching values");
            Console.WriteLine(rootObj.ToString());
        }

        [TestMethod]
        public void QueryJsonArray()
        {
            IJsonNode? root = JsonParser.StringToJsonNode(JsonString);
            Assert.IsNotNull(root, "Root node is null");
            object? query = root.Query("GlossSeeAlso");
            Assert.IsNotNull(query, "Query is null");
            Assert.IsInstanceOfType(query, typeof(JsonArray<object?>), "Queried object is not a JsonArray");
            JsonArray<object?> queryArr = (JsonArray<object?>)query;
            Assert.AreEqual(queryArr.Count, 2, "Queried JsonArray not the correct length");
            Console.WriteLine(queryArr.ToString());
        }
    }
}