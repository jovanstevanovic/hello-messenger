using System;
using System.Collections.Generic;
using System.Text;
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using System.Linq;

namespace BrzeBoljeJeftinije.Messenger.DB
{
    /*public class MockDBProvider : IDBProvider
    {
        private static List<User> users = new List<User>();
        private static List<Group> groups = new List<Group>();
        private static List<FriendRequest> friendRequests = new List<FriendRequest>();
        private static Dictionary<int, List<User>> usersByGroups = new Dictionary<int, List<User>>();
        private static Dictionary<int, List<Group>> groupsByUser = new Dictionary<int, List<Group>>();
        private static Dictionary<int, List<User>> friendships = new Dictionary<int, List<User>>();
        private static List<AdminUser> adminUsers = new List<AdminUser>();
        private static Dictionary<int, Dictionary<int, bool>> admins = new Dictionary<int, Dictionary<int, bool>>();
        private static int identity = 0;

        static MockDBProvider()
        {
            adminUsers.Add(new AdminUser
            {
                Username = "root",
                Password = "$"
            });
        }

        public void AddFriendship(User user1, User user2)
        {
            if (!friendships.ContainsKey(user1.Id)) friendships.Add(user1.Id, new List<User>());
            if (!friendships.ContainsKey(user2.Id)) friendships.Add(user2.Id, new List<User>());
            friendships[user1.Id].Add(user2);
            friendships[user2.Id].Add(user1);
        }

        public void AddUsersToGroup(List<User> users, Group group, bool isAdmin)
        {
            if (!usersByGroups.ContainsKey(group.Id)) usersByGroups.Add(group.Id, new List<User>());
            foreach(var user in users)
            {
                if (!groupsByUser.ContainsKey(user.Id)) groupsByUser.Add(user.Id, new List<Group>());
                usersByGroups[group.Id].Add(user);
                groupsByUser[user.Id].Add(group);
                if (!admins.ContainsKey(group.Id)) admins[group.Id] = new Dictionary<int, bool>();
                admins[group.Id][user.Id] = isAdmin;
            }
        }

        public bool CheckIfUserIsAdmin(User user, Group group)
        {
            throw new NotImplementedException();
        }

        public void CommitIfNecessary()
        {
            
        }

        public int CountUnreadMessages(User user, Group group)
        {
            throw new NotImplementedException();
        }

        public void CreateFriendRequest(FriendRequest request)
        {
            friendRequests.Add(request);
        }

        public Group CreateGroup(Group group)
        {
            groups.Add(group);
            identity++;
            group.Id = identity;
            return group;
        }

        public void DeleteFriendRequest(FriendRequest request)
        {
            friendRequests.RemoveAll(x => x.SenderId == request.SenderId && x.ReceiverId == request.ReceiverId);
        }

        public void DeleteFriendship(User user1, User user2)
        {
            if(friendships.ContainsKey(user1.Id))
            {
                friendships[user1.Id].RemoveAll(x => x.Id == user2.Id);
            }
            if (friendships.ContainsKey(user2.Id))
            {
                friendships[user2.Id].RemoveAll(x => x.Id == user1.Id);
            }
            var group = groups.FirstOrDefault(x => x.Binary && GetUsersInGroup(x).All(y=>y.Id==user1.Id || y.Id==user2.Id));
            if (group != null) DeleteGroup(group);
        }

        public void DeleteGroup(Group group)
        {
            var members = GetUsersInGroup(group);
            foreach(var member in members)
            {
                groupsByUser[member.Id].RemoveAll(x => x.Id == group.Id);
            }
            usersByGroups.Remove(group.Id);
            groups.RemoveAll(x => x.Id == group.Id);
        }

        public AdminUser GetAdminUser(string username)
        {
            return adminUsers.FirstOrDefault(x => x.Username == username);
        }

        public List<Attachment> GetAttachmentsForMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public MessageCryptoMaterial GetCryptographicMaterial(User user, Message message)
        {
            throw new NotImplementedException();
        }

        public List<User> GetFriends(User user)
        {
            var result=friendships.ContainsKey(user.Id)?friendships[user.Id]:new List<User>();
            return result.Select(x => users.First(y => y.Id == x.Id)).ToList();
        }

        public List<Group> GetGroupsForUser(User user)
        {
            if(groupsByUser.ContainsKey(user.Id))
            {
                var groups=groupsByUser[user.Id];
                groups.ForEach(x => x.IsAdmin = admins[x.Id][user.Id]);
                return groups;
            }
            else
            {
                return new List<Group>();
            }
        }

        public List<Message> GetMessagesForGroup(Group group, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public FriendRequest GetRequestBetween(User sender, User receiver)
        {
            return friendRequests.FirstOrDefault(x => x.SenderId == sender.Id && x.ReceiverId == receiver.Id);
        }

        public List<FriendRequest> GetSentFriendRequests(User user)
        {
            return friendRequests.Where(x => x.SenderId == user.Id).ToList();
        }

        public List<FriendRequest> GetUnresolvedFriendRequests(User user)
        {
            return friendRequests.Where(x => x.ReceiverId == user.Id && !x.Resolved).ToList();
        }

        public User GetUserByCertHash(string certHash, bool includePicture)
        {
            return users.FirstOrDefault(x => x.CertHash == certHash);
        }

        public User GetUserByID(int id, bool includePicture)
        {
            var user = users.FirstOrDefault(x => x.Id == id);
            return user;
        }

        public List<User> GetUsersInGroup(Group group)
        {
            var result=usersByGroups.ContainsKey(group.Id) ? usersByGroups[group.Id] : new List<User>();
            return result.Select(x => users.First(y => y.Id == x.Id)).ToList();
        }

        public void MarkMessageAsRead(User user, Message message)
        {
            throw new NotImplementedException();
        }

        public void RemoveUsersFromGroup(List<User> users, Group group)
        {
            foreach(var user in users)
            {
                groupsByUser[user.Id].RemoveAll(x => x.Id == group.Id);
                usersByGroups[group.Id].RemoveAll(x => x.Id == user.Id);
            }
        }

        public void RollbackIfNecessary()
        {
        }

        public List<User> SearchUsersByName(string name)
        {
            return users.Where(x => x.Name.ToUpper().Contains(name.ToUpper())).ToList();
        }

        public void StoreAdminUser(AdminUser user)
        {
            adminUsers.Add(user);
        }

        public void StoreAttachment(Attachment attachment)
        {
            throw new NotImplementedException();
        }

        public void StoreCryptographicMaterial(MessageCryptoMaterial material)
        {
            throw new NotImplementedException();
        }

        public void StoreMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public void StoreUser(User user)
        {
            user.Id = identity++;
            users.Add(user);
        }

        public void UpdateAdminUser(AdminUser user)
        {
            adminUsers.RemoveAll(x => x.Username == user.Username);
            adminUsers.Add(user);
        }

        public void UpdateFriendRequest(FriendRequest request)
        {
            friendRequests.RemoveAll(x => x.SenderId == request.SenderId && request.ReceiverId == x.ReceiverId);
            friendRequests.Add(request);
        }

        public void UpdateGroup(Group group, bool alsoUpdatePicture)
        {
            groups.RemoveAll(x => x.Id == group.Id);
            groups.Add(group);
        }

        public void UpdateUser(User user)
        {
            users.RemoveAll(x => x.Id == user.Id);
            users.Add(user);
        }
    }*/
}
