using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcWebApi.Models;

namespace MvcWebApi
{
    public class Db
    {
        //sues the singleton pattern
        private static Db _instance;
        private static object syncRoot = new Object();

        public ObjectSet<Product> Products; 
        private Db()
        {
            Products = new ObjectSet<Product>();
        }

        public static Db Instance
        {
            get {
                if (_instance == null)
                    lock (syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Db();
                            _instance.Products.Add(new List<Product>()
                            {
                                new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
                                new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
                                new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
                            });
                        }
                    }
                return _instance;
            }
        }
    }

    public class ObjectSet<T>
    {
        private static object syncRoot = new Object();
        private Dictionary<int, T> objects;

        public ObjectSet() { objects = new Dictionary<int, T>();}

        public List<T> Add(List<T> itemList)
        {
            foreach (var i in itemList)
            {
                Add(i);
            }
            return itemList;
        }
        public T Add(T item)
        {
            var key = item.GetHashCode();

            setIdFeild(item, key);
            objects[key] = item;
            return item;
        }

        private void setIdFeild(T item, int id)
        {
            //Uses reflextion to try to find id feild than sets it to the key 
            var type = item.GetType();

            var typeName = type.Name;

            var setMethod = type.GetMethod("set_Id") ?? type.GetMethod("set_" + typeName + "Id");
            //var getMethod = type.GetMethod("get_Id") ?? type.GetMethod("get_" + typeName + "Id");

            setMethod.Invoke(item, new object[] { id });
        }

        public bool Delete(int itemId)
        {
            if (objects.ContainsKey(itemId))
                lock (syncRoot)
                {
                    if (objects.ContainsKey(itemId))
                    {
                        objects.Remove(itemId);
                        return true;
                    }
                }
            return false;
        }

        public IEnumerable<T> GetEnumerable()
        {
            lock (syncRoot)
            {
                return objects.Values.ToList();
            }
        }

        public T Find(int itemId)
        {
            if (objects.Keys.Contains(itemId))
            {
                lock (syncRoot)
                {
                    if (objects.Keys.Contains(itemId))
                    {
                        return objects[itemId];
                    }
                }
            }
            return default(T);
        }


    }
}