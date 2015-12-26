﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UGoFor.API.Models;

namespace UGoFor.API.DAL
{
    public class CommentsDAL : BaseDAL
    {

        public List<CommentsModel> SelectAllPostComments()
        {
            List<CommentsModel> postComments = ExecuteSPReturnData<CommentsModel>("SelectAllPostComments");
            return postComments;
        }

        public List<CommentsModel> InsertComment(CommentsModel sentComment)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@POSTID", sentComment.PostId),
                new SqlParameter("@USERID", sentComment.UserId),
                new SqlParameter("@COMMENT", sentComment.Comment),
                new SqlParameter("@LOCATION", sentComment.Location),
            };

            List<CommentsModel> retset = ExecuteSPReturnData<CommentsModel>("InsertComment", parameters);

            return retset;
        }

        public static CommentsModel GetInitPost()
        {
            return new CommentsModel
            {
                Username = "ugofor",
                Comment = "hi",
                ProfileUrl = "img/craveprofile.jpg"
            };
        }
    }
}