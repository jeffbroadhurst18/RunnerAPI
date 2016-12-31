using Product_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace Album_API.Controllers
{
    public class AlbumController : ApiController
    {
        private string connectionString;

        public AlbumController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Albums"].ConnectionString;
        }

        // GET api/Album
        public List<Album> GetAllAlbums() //get method
        {
            //Instedd of static variable you can use database resource to get the data and return to API
            //return Albums.Value; //return all the Album list data
            return RetrieveAllAlbums();
        }

        // GET api/Album/5
        public IHttpActionResult GetAlbum(int id)
        {
            Album album = RetrieveAlbums(id);
            if (album == null)
            {
                return NotFound();
            }
            return Ok(album);
        }

        // POST api/Album
        public void AlbumAdd(Album Album) //post method
        {
            InsertAlbum(Album);
        }

            //DELETE api/Album/5
            [System.Web.Http.HttpDelete]
        public void Delete(int id)
        {
            RemoveAlbum(id);
        }
        
        private void InsertAlbum(Album album)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "insert into dbo.Albums (Artist,Title,Year) values (@Artist,@Title,@Year);";

                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@Artist", album.Artist);
                command.Parameters.AddWithValue("@Title", album.Title);
                command.Parameters.AddWithValue("@Year", album.Year);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private Album RetrieveAlbums(int id)
        {
            Album album = new Album();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select Artist, Title, Year,ID from dbo.Albums where id = @ID";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        album = new Album
                        {
                            Artist = reader[0].ToString(),
                            Title = reader[1].ToString(),
                            Year = (int)reader[2],
                            Id = (int)reader[3]
                        };

                        break;
                    }
                }

                connection.Close();
                return album;
            }
        }

        private List<Album> RetrieveAllAlbums()
        {
            var albumList = new List<Album>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select Artist,Title,Year,Id from dbo.Albums";
                SqlCommand command = new SqlCommand(sqlString, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Album Album = new Album
                        {
                            Artist = reader[0].ToString(),
                            Title = reader[1].ToString(),
                            Year = (int)reader[2],
                            Id = (int)reader[3]
                        };

                        albumList.Add(Album);
                    }
                }

                connection.Close();
                return albumList;
            }
        }

        private void RemoveAlbum(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "delete dbo.Albums where id=@ID";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@ID", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    
}
