using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AimsUtility.Orders
{
    /// <summary>
    /// A few helper methods for the Order class, which is effectively a structured
    /// json object.
    /// </summary>
    public static class JsonHelperMethods
    {
        /// <summary>
        /// Inserts a value into the Order object by json path, which is formatted
        /// like (one/two, value), where the key will be "two" underneath the 
        /// JObject "one" and the value will be whatever is put at value. It should
        /// be noted that this currently only inserts leaves, so objects like the
        /// address object are not supported. It should also be noted that this does
        /// not support inserting into arrays quite yet.
        /// </summary>
        /// <param name="Obj">The object to insert into</param>
        /// <param name="Path">The path to be inserted, of the format "one/two"</param>
        /// <param name="Value">The value to be inserted. Should be a primitive (e.g. string, double, etc.), otherwise there will be undefined behavior</param>
        public static void InsertValueByPath(this object Obj, string Path, object Value)
        {
            var pathArr = Path.Split("/");
            object targetObj = Obj;

            // iterate until we get to last element of the path, which
            // will be the key that we need to set a value to
            for(int i = 0; i < pathArr.Length - 1; i++)
            {
                // attempt to retrieve the next object from the target object
                var objField = targetObj.GetType().GetField(pathArr[i]);
                var nextObj = objField.GetValue(targetObj);

                // if it doesn't exist, we create a new object for the current target object
                if(nextObj == null)
                {
                    var objDefaultConstructor = objField.FieldType.GetConstructor(Type.EmptyTypes);
                    nextObj = objDefaultConstructor.Invoke(Type.EmptyTypes);
                    objField.SetValue(targetObj, nextObj);
                }

                // we set targetObj to what we just created/found, iterating the process another layer
                targetObj = nextObj;
            }

            // get the field where we insert the value (e.g. customerID)
            var keyField = targetObj.GetType().GetField(pathArr[pathArr.Length - 1]);

            // insert the value at that field
            keyField.SetValue(targetObj, Value);
        }

        /// <summary>
        /// Gets the value of a property or subproperty of an object using
        /// Newtonsoft's SelectToken syntax. Simply does this by converting
        /// it to JObject then selecting
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static object GetValueByPath(this object Obj, string Path)
        {
            var jsonObj = (JObject)JToken.FromObject(Obj);
            return jsonObj.SelectToken(Path);
        }

    }
}