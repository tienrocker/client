#if !UNITY_5_3_OR_NEWER
using Photon.LoadBalancing.Custom.Models;
using Photon.LoadBalancing.Custom.Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Photon.LoadBalancing.Custom.Server.Data
{
    public class UserDB
    {
        private static object lockUserobj = new object();

        public static List<ModelUser> Users = new List<ModelUser>();

        public static ModelUser GetUserByUsername(string username)
        {
            lock (lockUserobj)
            {
                var item = Users.Find(x => x.username == username);
                if (item != null) return item;

                using (var db = new DbContext())
                {
                    item = db.Users.FirstOrDefault(acc => acc.username == username);
                    if (item != null)
                    {
                        Users.Add(item);
                        return item;
                    }
                    return null;
                }
            }
        }

        public static ModelUser GetUserById(int id)
        {
            lock (lockUserobj)
            {
                var item = Users.Find(x => x.id == id);
                if (item != null) return item;

                using (var db = new DbContext())
                {
                    item = db.Users.FirstOrDefault(acc => acc.id == id);
                    if (item != null)
                    {
                        Users.Add(item);
                        return item;
                    }
                    return null;
                }
            }
        }

        public static int AddUser(ModelUser item)
        {
            if (item == null || String.IsNullOrEmpty(item.username) || String.IsNullOrEmpty(item.password)) return 0;

            lock (lockUserobj)
            {
                using (var db = new DbContext())
                {
                    db.Users.Add(item);
                    return db.SaveChanges();
                }
            }
        }
    }
}
#endif