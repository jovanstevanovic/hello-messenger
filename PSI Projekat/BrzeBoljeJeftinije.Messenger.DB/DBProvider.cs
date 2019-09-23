/*
 * DBProvider.cs
 * Autori:
 * Marko Milivojevic 2015/0481
 * Uros Milivojevic 2015/0603
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BrzeBoljeJeftinije.Messenger.DB;
using BrzeBoljeJeftinije.Messenger.DB.Entities;
using System.Configuration;

namespace BrzeBoljeJeftinije.Messenger.DB
{
    /*
     * Klasa za komunikaciju aplikacije sa bazom</summary>
     *
     * <remarks>verzija 1.0</remarks>
    */
    public class DBProvider : IDBProvider
    {
        private static readonly string conString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["ConnectionString"]].ConnectionString;
        private SqlConnection con = null;
        private SqlTransaction trans = null;

        /*
         * <summary>Postavljanje konekcije sa bazom</summary>
         *
         * <returns>SqlTransaction</returns>
        */
        private SqlTransaction GetTransaction()
        {
            if (con == null)
            {
                con = new SqlConnection(conString);
                con.Open();
            }
            if (trans == null)
            {
                trans = con.BeginTransaction();
            }
            return trans;
        }

        /*
         * <summary>Dodavanje prijatelja</summary>
         *
         * <param name="user1"></param>
         * <param name="user2"></param>
         *
         * <returns>void</returns>
        */
        public void AddFriendship(User user1, User user2)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_AddFriendship";

            cmd.Parameters.Add(new SqlParameter("@User1", user1.Id));
            cmd.Parameters.Add(new SqlParameter("@User2", user2.Id));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Dodavanje prijatelja u grupu</summary>
         *
         * <param name="user"></param>
         * <param name="group"></param>
         * <param name="isAdmin"></param>
         *
         * <returns>void</returns>
        */
        public void AddUsersToGroup(List<User> user, Group group, bool isAdmin)
        {

            SqlCommand cmd = new SqlCommand("exec dbo.sp_AddUsersToGroup @list, @GroupId, @IsAdmin, @Iterator", con);
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            using (var table = new DataTable())
            {
                table.Columns.Add("User", typeof(int));

                for (int i = 0; i < user.Count; i++)
                    table.Rows.Add(user[i].Id);

                var pList = new SqlParameter("@list", SqlDbType.Structured);
                pList.TypeName = "dbo.ListOfUsers";
                pList.Value = table;

                cmd.Parameters.Add(pList);
            }




            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            cmd.Parameters.Add(new SqlParameter("@IsAdmin", isAdmin));
            cmd.Parameters.Add(new SqlParameter("@Iterator", user.Count - 1));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Provera da li je trenutni user admin grupe</summary>
         *
         * <param name="user"></param>
         * <param name="group"></param>
         *
         * <returns>bool</returns>
        */
        public bool CheckIfUserIsAdmin(User user, Group group)
        {
            bool result = false;


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select GM.IsAdmin" +
                                 " From dbo.GroupMember GM" +
                                 " Where GM.GroupId = @GroupId " +
                                 " AND GM.UserId = @UserId ";
            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    result = (bool)reader["IsAdmin"];
                }
                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>izvrsavanje transakcije</summary>
         *
         * <returns>void</returns>
        */
        public void CommitIfNecessary()
        {
            try
            {
                if (trans != null)
                {
                    trans.Commit();
                }
            }
            finally
            {
                try
                {
                    if (con != null && con.State != ConnectionState.Closed) con.Close();
                }
                finally
                {
                    con = null;
                    trans = null;
                }
            }
        }

        /*
         * <summary>Brojanje neprocitanih poruka u nekoj grupi, za nekog korisnika</summary>
         *
         * <param name="user"></param>
         * <param name="group"></param>
         *
         * <returns>int</returns>
        */
        public int CountUnreadMessages(User user, Group group)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select Count(*)" +
                                 " From dbo.Message M" +
                                 " Where M.GroupId = @GroupId " +
                                 " AND M.MessageId NOT IN " +
                                                        "(Select MessageId" +
                                                        " From dbo.MessageRead MR" +
                                                        " Where MR.UserId = @UserId)";
            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));

            int result = (int)cmd.ExecuteScalar();

            return result;
        }

        /*
         * <summary>Kreiranje zahteva za prijateljstvo</summary>
         *
         * <param name="request"></param>
         *
         * <returns>void</returns>
        */
        public void CreateFriendRequest(FriendRequest request)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_CreateFriendRequest";

            cmd.Parameters.Add(new SqlParameter("@SenderId", request.SenderId));
            cmd.Parameters.Add(new SqlParameter("@ReceiverId", request.ReceiverId));
            cmd.Parameters.Add(new SqlParameter("@Resolved", request.Resolved));
            cmd.Parameters.Add(new SqlParameter("@Seen", request.Seen));
            cmd.Parameters.Add(new SqlParameter("@TimeStamp", request.Timestamp));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Kreiranje grupe</summary>
         *
         * <param name="group"></param>
         *
         * <returns>Group</returns>
        */
        public Group CreateGroup(Group group)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_CreateGroup";

            cmd.Parameters.Add(new SqlParameter("@Name", group.Name));
            cmd.Parameters.Add(new SqlParameter("@TimeStamp", group.Timestamp));
            if (group.Picture != null)
            {
                cmd.Parameters.Add(new SqlParameter("@Picture", group.Picture));
                cmd.Parameters.Add(new SqlParameter("@PictureType", group.PictureType));
            }
            cmd.Parameters.Add(new SqlParameter("@IsBinary", group.Binary));

            group.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return group;
        }

        /*
         * <summary>Brisanje zahteva za prijateljstvo</summary>
         *
         * <param name="request"></param>
         *
         * <returns>void</returns>
        */
        public void DeleteFriendRequest(FriendRequest request)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_DeleteFriendRequest";

            cmd.Parameters.Add(new SqlParameter("@SenderId", request.SenderId));
            cmd.Parameters.Add(new SqlParameter("@ReceiverId", request.ReceiverId));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Brisanje prijateljstva izmedju dva korisnika</summary>
         *
         * <param name="user1"></param>
         * <param name="user2"></param>
         *
         * <returns>void</returns>
        */
        public void DeleteFriendship(User user1, User user2)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_DeleteFriendship";

            cmd.Parameters.Add(new SqlParameter("@User1Id", user1.Id));
            cmd.Parameters.Add(new SqlParameter("@User2Id", user2.Id));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Brisanje grupe</summary>
         *
         * <param name="group"></param>
         *
         * <returns>void</returns>
        */
        public void DeleteGroup(Group group)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_DeleteGroup";

            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Brisanje korisnika</summary>
         *
         * <param name="user"></param>
         *
         * <returns>void</returns>
        */
        public void DeleteUser(User user)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_DeleteUser";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Dohvatanje administratora na osnovu imena</summary>
         *
         * <param name="username"></param>
         *
         * <returns>AdminUser</returns>
        */
        public AdminUser GetAdminUser(string username)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select AdminUserId, Password" +
                                 " From [dbo].[AdminUser]" +
                                 " Where Username = @Username";

            cmd.Parameters.Add(new SqlParameter("@Username", username));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                AdminUser result = new AdminUser();

                if (reader.HasRows)
                {
                    reader.Read();
                    result.Id = (int)reader["AdminUserId"];
                    result.Username = username;
                    result.Password = reader["Password"].ToString();
                }
                else
                    return null;

                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje priloga neke poruke</summary>
         *
         * <param name="message"></param>
         *
         * <returns>List<Attachment></returns>
        */
        public List<Attachment> GetAttachmentsForMessage(Message message)
        {
            List<Attachment> attachments = new List<Attachment>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select AttachmentId, FileName, FileExtension, Content" +
                                 " From dbo.Attachment" +
                                 " Where MessageId = @MessageId";

            cmd.Parameters.Add(new SqlParameter("@MessageId", message.Id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Attachment attachment = new Attachment();

                        attachment.Id = (int)reader["AttachmentId"];
                        attachment.MessageId = message.Id;
                        attachment.FileName = reader["FileName"].ToString();
                        attachment.FileExtension = reader["FileExtension"].ToString();
                        attachment.Content = (byte[])reader["Content"];

                        attachments.Add(attachment);
                    }
                }

                return attachments;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje kriptovanog materijala neke poruke</summary>
         *
         * <param name="user"></param>
         * <param name="message"></param>
         *
         * <returns>MessageCryptoMaterial</returns>
        */
        public MessageCryptoMaterial GetCryptographicMaterial(User user, Message message)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select MessageCryptoMaterialId, Material" +
                                 " From dbo.MessageCryptoMaterial" +
                                 " Where UserId = @UserId " +
                                 " AND MessageId = @MessageId ";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            cmd.Parameters.Add(new SqlParameter("@MessageId", message.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                MessageCryptoMaterial material = new MessageCryptoMaterial();
                if (reader.HasRows)
                {
                    reader.Read();
                    material.Id = (int)reader["MessageCryptoMaterialId"];
                    material.Material = reader["Material"].ToString();
                    material.UserId = user.Id;
                    material.MessageId = message.Id;
                }
                else
                    return null;

                return material;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje liste prijatelja</summary>
         *
         * <param name="user"></param>
         *
         * <returns>List<User></returns>
        */
        public List<User> GetFriends(User user)
        {
            List<User> userFriends = new List<User>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select *" +
                              " From [dbo].[User] U" +
                              " Join [dbo].[Friendship] F " +
                              " On ((F.User2 = @UserId And F.User1 = U.UserId) Or (F.User1 = @UserId And F.User2 = U.UserId))";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User us = new User();
                        us.Id = (int)reader["Userid"];
                        us.Name = reader["Name"].ToString();
                        us.CertHash = reader["CertHash"].ToString();
                        us.Certificate = (byte[])reader["Certificate"];

                        if (reader["Picture"] != System.DBNull.Value)
                        {
                            us.Picture = (byte[])reader["Picture"];
                            us.PictureType = reader["PictureType"].ToString();
                        }
                        else
                        {
                            us.Picture = null;
                            us.PictureType = null;
                        }

                        if (reader["RtID"] != System.DBNull.Value)
                        {
                            us.RtID = reader["RtID"].ToString();
                        }
                        else
                        {
                            us.BannedUntil = null;
                        }

                        if (reader["BannedUntil"] != System.DBNull.Value)
                        {
                            us.BannedUntil = (DateTime)reader["BannedUntil"];
                        }
                        else
                        {
                            us.BannedUntil = null;
                        }

                        userFriends.Add(us);
                    }

                }

                return userFriends;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje grupe preko id-a</summary>
         *
         * <param name="id"></param>
         *
         * <returns>Group</returns>
        */
        public Group GetGroupById(int id)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select *" +
                                 " From [Group]" +
                                 " Where GroupId = @GroupId";
            cmd.Parameters.Add(new SqlParameter("@GroupId", id));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                Group group = new Group();
                if (reader.HasRows)
                {
                    reader.Read();
                    group.Id = (int)reader["GroupId"];
                    group.Name = reader["Name"].ToString();
                    group.Timestamp = (DateTime)reader["TimeStamp"];
                    if (reader["Picture"] != System.DBNull.Value)
                    {
                        group.Picture = (byte[])reader["Picture"];
                        group.PictureType = reader["PictureType"].ToString();
                    }
                    else
                    {
                        group.Picture = null;
                        group.PictureType = null;
                    }
                    group.Binary = (bool)reader["IsBinary"];
                }
                else
                {
                    return null;
                }

                return group;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje liste grupa nekog korisnika</summary>
         *
         * <param name="user"></param>
         *
         * <returns>List<Group></returns>
        */
        public List<Group> GetGroupsForUser(User user)
        {
            List<Group> groups = new List<Group>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select G.GroupId as 'GroupId', G.Name as 'Name', G.Picture as 'Picture', G.PictureType as 'PictureType', G.IsBinary as 'IsBinary', GM.IsAdmin as 'IsAdmin', G.TimeStamp as 'TimeStamp', " +
                "(SELECT COUNT(*) FROM [dbo].[Message] M " +
                "WHERE M.GroupId=G.GroupId " +
                "AND M.SenderId!=@UserId A" +
                "ND NOT EXISTS (SELECT * FROM dbo.MessageRead MR WHERE MR.MessageId=M.MessageId AND MR.UserId=@UserId)) as 'Unread', " +
                "(SELECT MAX(MS.Datetime) FROM dbo.Message MS WHERE Ms.GroupId=G.GroupId) AS 'LastMessage' " +
                              " From [dbo].[Group] G" +
                              " Join [dbo].[GroupMember] GM On (GM.UserId = @UserId AND G.GroupId = GM.GroupId)";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Group group = new Group();
                        group.Id = (int)reader["GroupId"];
                        group.Name = reader["Name"].ToString();
                        group.Timestamp = (DateTime)reader["TimeStamp"];
                        if (reader["Picture"] != System.DBNull.Value)
                        {
                            group.Picture = (byte[])reader["Picture"];
                            group.PictureType = reader["PictureType"].ToString();
                        }
                        else
                        {
                            group.Picture = null;
                            group.PictureType = null;
                        }
                        group.Binary = (bool)reader["IsBinary"];    //mozda treba neka druga konverzija jer reader vraca 0 ili 1
                        group.IsAdmin = (bool)reader["IsAdmin"];
                        group.ContainsUnread = ((int)reader["Unread"]) > 0;
                        group.LastMessage = (reader["LastMessage"] != DBNull.Value ? (DateTime?)reader["LastMessage"] : group.Timestamp);
                        groups.Add(group);
                    }
                }

                return groups;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje poruke preko id-a</summary>
         *
         * <param name="id"></param>
         *
         * <returns>Message</returns>
        */
        public Message GetMessageById(int id)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select *" +
                                 " From [Message]" +
                                 " Where MessageId = @MessageId";

            cmd.Parameters.Add(new SqlParameter("@MessageId", id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                Message message = new Message();
                if (reader.HasRows)
                {
                    reader.Read();
                    message.Id = (int)reader["MessageId"];
                    message.GroupId = (int)reader["GroupId"];
                    message.SenderId = (int)reader["SenderId"];
                    message.TimeStamp = (DateTime)reader["Datetime"];
                    message.Text = reader["Text"].ToString();
                }
                else
                {
                    return null;
                }

                return message;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje odredjenog broja poruka iz grupe</summary>
         *
         * <param name="group"></param>
         * <param name="pageNumber"></param>
         * <param name="pageSize"></param>
         * <param name="user"></param>
         *
         * <returns>List<Message></returns>
        */
        public List<Message> GetMessagesForGroup(Group group, int pageNumber, int pageSize, User user = null)
        {
            List<Message> result = new List<Message>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_GetMessagesForGroup";
            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            cmd.Parameters.Add(new SqlParameter("@StartRow", pageNumber * pageSize));
            cmd.Parameters.Add(new SqlParameter("@EndRow", pageNumber * pageSize + pageSize));
            if (user != null)
            {
                cmd.Parameters.Add(new SqlParameter("@User", user.Id));
            }
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Message message = new Message();
                        message.Id = (int)reader["MessageId"];
                        message.GroupId = (int)reader["GroupId"];
                        message.SenderId = (int)reader["SenderId"];
                        message.TimeStamp = (DateTime)reader["DateTime"];
                        message.Text = reader["Text"].ToString();

                        result.Add(message);
                    }
                }

                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje zahteva za prijateljstvo izmedju dva korisnika</summary>
         *
         * <param name="sender"></param>
         * <param name="receiver"></param>
         *
         * <returns>FriendRequest</returns>
        */
        public FriendRequest GetRequestBetween(User sender, User receiver)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select Resolved, Seen" +
                                 " From dbo.FriendRequest" +
                                 " Where SenderId = @SenderId " +
                                 " AND ReceiverId = @ReceiverId";

            cmd.Parameters.Add(new SqlParameter("@SenderId", sender.Id));
            cmd.Parameters.Add(new SqlParameter("@ReceiverId", receiver.Id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                FriendRequest friendRequest = new FriendRequest();

                if (reader.HasRows)
                {
                    reader.Read();
                    friendRequest.SenderId = sender.Id;
                    friendRequest.ReceiverId = receiver.Id;
                    friendRequest.Resolved = (bool)reader["Resolved"];
                    friendRequest.Seen = (bool)reader["Seen"];
                }
                else
                    return null;

                return friendRequest;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje poslatih zahteva za prijateljstvo odredjenog korisnika</summary>
         *
         * <param name="user"></param>
         *
         * <returns>List<FriendRequest></returns>
        */
        public List<FriendRequest> GetSentFriendRequests(User user)
        {
            List<FriendRequest> friendRequests = new List<FriendRequest>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select ReceiverId, Resolved, Seen, TimeStamp" +
                                 " From dbo.FriendRequest" +
                                 " Where SenderId = @UserId";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        FriendRequest friendRequest = new FriendRequest();

                        friendRequest.SenderId = user.Id;
                        friendRequest.ReceiverId = (int)reader["ReceiverId"];
                        friendRequest.Resolved = (bool)reader["Resolved"];
                        friendRequest.Seen = (bool)reader["Seen"];
                        friendRequest.Timestamp = (DateTime)reader["TimeStamp"];

                        friendRequests.Add(friendRequest);
                    }

                }

                return friendRequests;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje neresenih zahteva za prijateljstvo odredjenog korisnika</summary>
         *
         * <param name="user"></param>
         *
         * <returns>List<FriendRequest></returns>
        */
        public List<FriendRequest> GetUnresolvedFriendRequests(User user)
        {
            List<FriendRequest> unresolvedFriendRequests = new List<FriendRequest>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select SenderId, Seen, TimeStamp" +
                                 " From dbo.FriendRequest" +
                                 " Where Resolved = 0 And ReceiverId = @UserId";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        FriendRequest friendRequest = new FriendRequest();
                        friendRequest.SenderId = (int)reader["SenderId"];
                        friendRequest.ReceiverId = user.Id;
                        friendRequest.Resolved = false;
                        friendRequest.Seen = (bool)reader["Seen"];
                        friendRequest.Timestamp = (DateTime)reader["TimeStamp"];

                        unresolvedFriendRequests.Add(friendRequest);
                    }

                }
                else
                    return new List<FriendRequest>();

                return unresolvedFriendRequests;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje korisnika na osnovu serijskog broja sertifikata</summary>
         *
         * <param name="certHash"></param>
         * <param name="includePicture"></param>
         *
         * <returns>User</returns>
        */
        public User GetUserByCertHash(string certHash, bool includePicture)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            if (includePicture)
                cmd.CommandText = "Select *" +
                                 " From [User]" +
                                 " Where CertHash = @CertHash";
            else
                cmd.CommandText = "Select UserId, Name, CertHash, Certificate, RtId, BannedUntil" +
                                 " From [User]" +
                                 " Where CertHash = @CertHash";
            cmd.Parameters.Add(new SqlParameter("@CertHash", certHash));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                User user = new User();
                if (reader.HasRows)
                {
                    reader.Read();
                    user.Id = (int)reader["UserId"];
                    user.Name = reader["Name"].ToString();
                    user.CertHash = reader["CertHash"].ToString();
                    user.Certificate = (byte[])reader["Certificate"];

                    if (includePicture)
                    {
                        user.Picture = (byte[])reader["Picture"];
                        user.PictureType = reader["PictureType"].ToString();
                    }
                    else
                    {
                        user.Picture = null;
                        user.PictureType = null;
                    }

                    if (reader["RtID"] != System.DBNull.Value)
                    {
                        user.RtID = reader["RtID"].ToString();
                    }
                    else
                    {
                        user.BannedUntil = null;
                    }

                    if (reader["BannedUntil"] != System.DBNull.Value)
                    {
                        user.BannedUntil = (DateTime)reader["BannedUntil"];
                    }
                    else
                    {
                        user.BannedUntil = null;
                    }
                }
                else
                    return null;

                return user;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje korisnika na osnovu id-a</summary>
         *
         * <param name="id"></param>
         * <param name="includePicture"></param>
         *
         * <returns>User</returns>
        */
        public User GetUserByID(int id, bool includePicture)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            if (includePicture)
                cmd.CommandText = "Select *" +
                                 " From [User]" +
                                 " Where UserId = @UserId";
            else
                cmd.CommandText = "Select UserId, Name, CertHash, Certificate, RtId, BannedUntil" +
                                 " From [User]" +
                                 " Where UserId = @UserId";

            cmd.Parameters.Add(new SqlParameter("@UserId", id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                User user = new User();
                if (reader.HasRows)
                {
                    reader.Read();
                    user.Id = id;
                    user.Name = reader["Name"].ToString();
                    user.CertHash = reader["CertHash"].ToString();
                    user.Certificate = (byte[])reader["Certificate"];
                    if (includePicture)
                    {
                        user.Picture = (byte[])reader["Picture"];
                        user.PictureType = reader["PictureType"].ToString();
                    }
                    else
                    {
                        user.Picture = null;
                        user.PictureType = null;
                    }

                    if (reader["RtID"] != System.DBNull.Value)
                    {
                        user.RtID = reader["RtID"].ToString();
                    }
                    else
                    {
                        user.BannedUntil = null;
                    }

                    if (reader["BannedUntil"] != System.DBNull.Value)
                    {
                        user.BannedUntil = (DateTime)reader["BannedUntil"];
                    }
                    else
                    {
                        user.BannedUntil = null;
                    }
                }
                else
                    return null;

                return user;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Dohvatanje liste korisnika neke grupe</summary>
         *
         * <param name="group"></param>
         *
         * <returns>List<User></returns>
        */
        public List<User> GetUsersInGroup(Group group)
        {
            List<User> users = new List<User>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select *" +
                              " From [dbo].[User] U" +
                              " Join [dbo].[GroupMember] GM On (GM.GroupId = @GroupId AND U.UserId = GM.UserId)";

            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User user = new User();

                        user.Id = (int)reader["Userid"];
                        user.Name = reader["Name"].ToString();
                        user.CertHash = reader["CertHash"].ToString();
                        user.Certificate = (byte[])reader["Certificate"];
                        if (reader["Picture"] != System.DBNull.Value)
                        {
                            user.Picture = (byte[])reader["Picture"];
                            user.PictureType = reader["PictureType"].ToString();
                        }
                        else
                        {
                            user.Picture = null;
                            user.PictureType = null;
                        }

                        if (reader["RtID"] != System.DBNull.Value)
                        {
                            user.RtID = reader["RtID"].ToString();
                        }
                        else
                        {
                            user.BannedUntil = null;
                        }

                        if (reader["BannedUntil"] != System.DBNull.Value)
                        {
                            user.BannedUntil = (DateTime)reader["BannedUntil"];
                        }
                        else
                        {
                            user.BannedUntil = null;
                        }

                        users.Add(user);
                    }

                }

                return users;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Oznacavanje poruke da je procitana</summary>
         *
         * <param name="user"></param>
         * <param name="message"></param>
         *
         * <returns>void</returns>
        */
        public void MarkMessageAsRead(User user, Message message)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreReadMessage";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            cmd.Parameters.Add(new SqlParameter("@MessageId", message.Id));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Brisanje liste korisnika iz grupe</summary>
         *
         * <param name="users"></param>
         * <param name="group"></param>
         *
         * <returns>void</returns>
        */
        public void RemoveUsersFromGroup(List<User> users, Group group)
        {
            SqlCommand cmd = new SqlCommand("exec dbo.sp_RemoveUsersFromGroup @list, @GroupId, @Iterator", con);
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            using (var table = new DataTable())
            {
                table.Columns.Add("User", typeof(int));

                for (int i = 0; i < users.Count; i++)
                    table.Rows.Add(users[i].Id);

                var pList = new SqlParameter("@list", SqlDbType.Structured);
                pList.TypeName = "dbo.ListOfUsers";
                pList.Value = table;

                cmd.Parameters.Add(pList);
            }
            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            cmd.Parameters.Add(new SqlParameter("@Iterator", users.Count - 1));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>vracanje transakcije ako je neophodno</summary>
         *
         * <returns>void</returns>
        */
        public void RollbackIfNecessary()
        {
            try
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
            }
            finally
            {
                if (con != null && con.State != ConnectionState.Closed) con.Close();
                con = null;
                trans = null;
            }
        }

        /*
         * <summary>Pretraga korisnika po imenu</summary>
         *
         * <param name="name"></param>
         *
         * <returns>List<User></returns>
        */
        public List<User> SearchUsersByName(string name)
        {
            List<User> users = new List<User>();


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select UserId, Name, CertHash, Certificate, Picture, PictureType, RtId, BannedUntil" +
                                 " From [dbo].[User]" +
                                 " Where Name like @Name";
            name = "%" + name + "%";
            cmd.Parameters.Add(new SqlParameter("@Name", name));

            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.Id = (int)reader["Userid"];
                        user.Name = reader["Name"].ToString();
                        user.CertHash = reader["CertHash"].ToString();
                        user.Certificate = (byte[])reader["Certificate"];

                        if (reader["Picture"] != System.DBNull.Value)
                        {
                            user.Picture = (byte[])reader["Picture"];
                            user.PictureType = reader["PictureType"].ToString();
                        }
                        else
                        {
                            user.Picture = null;
                            user.PictureType = null;
                        }

                        if (reader["RtID"] != System.DBNull.Value)
                        {
                            user.RtID = reader["RtID"].ToString();
                        }
                        else
                        {
                            user.BannedUntil = null;
                        }

                        if (reader["BannedUntil"] != System.DBNull.Value)
                        {
                            user.BannedUntil = (DateTime)reader["BannedUntil"];
                        }
                        else
                        {
                            user.BannedUntil = null;
                        }

                        users.Add(user);
                    }
                }

                return users;
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Ubacivanje administratora u bazu</summary>
         *
         * <param name="user"></param>
         *
         * <returns>void</returns>
        */
        public void StoreAdminUser(AdminUser user)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreAdminUser";

            cmd.Parameters.Add(new SqlParameter("@Username", user.Username));
            cmd.Parameters.Add(new SqlParameter("@Password", user.Password));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Ubacivanje priloga u bazu</summary>
         *
         * <param name="attachment"></param>
         *
         * <returns>void</returns>
        */
        public void StoreAttachment(Attachment attachment)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreAttachment";

            cmd.Parameters.Add(new SqlParameter("@FileName", attachment.FileName));
            cmd.Parameters.Add(new SqlParameter("@FileExtension", attachment.FileExtension));
            cmd.Parameters.Add(new SqlParameter("@Content", attachment.Content));
            cmd.Parameters.Add(new SqlParameter("@MessageId", attachment.MessageId));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Ubacivanje kriptovanog materijala u bazu</summary>
         *
         * <param name="material"></param>
         *
         * <returns>void</returns>
        */
        public void StoreCryptographicMaterial(MessageCryptoMaterial material)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreMessageCryptoMaterial";

            cmd.Parameters.Add(new SqlParameter("@Material", material.Material));
            cmd.Parameters.Add(new SqlParameter("@MessageId", material.MessageId));
            cmd.Parameters.Add(new SqlParameter("@UserId", material.UserId));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Ubacivanje poruke u bazu</summary>
         *
         * <param name="message"></param>
         *
         * <returns>void</returns>
        */
        public void StoreMessage(Message message)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreMessage";

            cmd.Parameters.Add(new SqlParameter("@GroupId", message.GroupId));
            cmd.Parameters.Add(new SqlParameter("@SenderId", message.SenderId));
            cmd.Parameters.Add(new SqlParameter("@DateTime", message.TimeStamp));
            cmd.Parameters.Add(new SqlParameter("@Text", message.Text));

            message.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        /*
         * <summary>Ubacivanje korisnika u bazu</summary>
         *
         * <param name="user"></param>
         *
         * <returns>void</returns>
        */
        public void StoreUser(User user)
        {


            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_StoreUser";

            cmd.Parameters.Add(new SqlParameter("@Name", user.Name));
            cmd.Parameters.Add(new SqlParameter("@CertHash", user.CertHash));
            cmd.Parameters.Add(new SqlParameter("@Certificate", user.Certificate));

            if (user.Picture != null)
            {
                cmd.Parameters.Add(new SqlParameter("@Picture", user.Picture));
                cmd.Parameters.Add(new SqlParameter("@PictureType", user.PictureType));
            }

            if (user.RtID != null)
            {
                cmd.Parameters.Add(new SqlParameter("@RtId", user.RtID));
            }

            if (user.BannedUntil == null)
            {
                cmd.Parameters.Add(new SqlParameter("@BannedUntil", user.BannedUntil));
            }

            user.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        /*
         * <summary>Promena osobina administratora</summary>
         *
         * <param name="user"></param>
         *
         * <returns>void</returns>
        */
        public void UpdateAdminUser(AdminUser user)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_UpdateAdminUser";

            cmd.Parameters.Add(new SqlParameter("@AdminUserId", user.Id));
            cmd.Parameters.Add(new SqlParameter("@Username", user.Username));
            cmd.Parameters.Add(new SqlParameter("@Password", user.Password));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Promena osobina zahteva za prijateljstvo</summary>
         *
         * <param name="request"></param>
         *
         * <returns>void</returns>
        */
        public void UpdateFriendRequest(FriendRequest request)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_UpdateFriendRequest";

            cmd.Parameters.Add(new SqlParameter("@SenderId", request.SenderId));
            cmd.Parameters.Add(new SqlParameter("@ReceiverId", request.ReceiverId));
            cmd.Parameters.Add(new SqlParameter("@Resolved", request.Resolved));
            cmd.Parameters.Add(new SqlParameter("@Seen", request.Seen));

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Promena osobina grupe</summary>
         *
         * <param name="group"></param>
         * <param name="alsoUpdatePicture"></param>
         *
         * <returns>void</returns>
        */
        public void UpdateGroup(Group group, bool alsoUpdatePicture)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_UpdateGroup";

            cmd.Parameters.Add(new SqlParameter("@GroupId", group.Id));
            cmd.Parameters.Add(new SqlParameter("@Name", group.Name));
            cmd.Parameters.Add(new SqlParameter("@TimeStamp", group.Timestamp));
            cmd.Parameters.Add(new SqlParameter("@IsBinary", group.Binary));
            cmd.Parameters.Add(new SqlParameter("@Picture", group.Picture));
            cmd.Parameters.Add(new SqlParameter("@PictureType", group.PictureType));
            cmd.Parameters.Add(new SqlParameter("@AlsoUpdatePicture", alsoUpdatePicture));

            cmd.ExecuteNonQuery();

        }

        /*
         * <summary>Promena osobina korisnika</summary>
         *
         * <param name="user"></param>
         *
         * <returns>void</returns>
        */
        public void UpdateUser(User user)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_UpdateUser";

            cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
            cmd.Parameters.Add(new SqlParameter("@Name", user.Name));
            cmd.Parameters.Add(new SqlParameter("@CertHash", user.CertHash));
            cmd.Parameters.Add(new SqlParameter("@Certificate", user.Certificate));
            if (user.Picture != null)
            {
                cmd.Parameters.Add(new SqlParameter("@Picture", user.Picture));
                cmd.Parameters.Add(new SqlParameter("@PictureType", user.PictureType));
            }
            if (user.RtID != null)
            {
                cmd.Parameters.Add(new SqlParameter("@RtId", user.RtID));
            }
            if (user.BannedUntil != null)
            {
                cmd.Parameters.Add(new SqlParameter("@BannedUntil", user.BannedUntil));
            }

            cmd.ExecuteNonQuery();
        }

        /*
         * <summary>Dohvatanje priloga preko id-a</summary>
         *
         * <param name="id"></param>
         *
         * <returns>Attachment</returns>
        */
        public Attachment GetAttachmentById(int id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select *" +
                                 " From [dbo].[Attachment]" +
                                 " Where [AttachmentId] = @Id";

            cmd.Parameters.Add(new SqlParameter("@Id", id));
            SqlDataReader reader = cmd.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Attachment attachment = new Attachment();

                        attachment.Id = (int)reader["AttachmentId"];
                        attachment.MessageId = (int)reader["MessageId"];
                        attachment.FileName = reader["FileName"].ToString();
                        attachment.FileExtension = reader["FileExtension"].ToString();
                        attachment.Content = (byte[])reader["Content"];

                        return attachment;
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                reader.Close();
            }
        }

        /*
         * <summary>Brisanje poruke preko id-a</summary>
         *
         * <param name="id"></param>
         *
         * <returns>void</returns>
        */
        public void DeleteMessage(int id)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Transaction = GetTransaction(); cmd.Connection = GetTransaction().Connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE FROM [dbo].[Message] WHERE [MessageId]=@Id";

            cmd.Parameters.Add(new SqlParameter("@Id", id));
            cmd.ExecuteNonQuery();
        }
    }
}
