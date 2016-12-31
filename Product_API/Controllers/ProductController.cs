using Product_API.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Product_API.Controllers
{
    public class ProductController : ApiController
    {

        public static Lazy<List<Product>> products = new Lazy<List<Product>>();//Static variable use only for demo, don’t use unless until require in project. 
        public static int PageLoadFlag = 1; // Page load count. 
        public static int ProductID = 4;
        private string connectionString;

        public ProductController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
        }

        // GET api/product
        public List<Product> GetAllProducts() //get method
        {
            //Instedd of static variable you can use database resource to get the data and return to API
            //return products.Value; //return all the product list data
            return RetrieveAllProducts();
        }

        // GET api/product/5
        public IHttpActionResult GetProduct(int id)
        {
            Product product = RetrieveProducts(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST api/product
        public void ProductAdd(Product product) //post method
        {
            //product.ID = ProductID;
            //products.Value.Add(product); //add the post product data to the product list
            //ProductID++;
            //instead of adding product data to the static product list you can save data to the database.
            InsertProduct(product);
        }

                private void InsertProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "insert into dbo.Products (Name,Type,Price) values (@Name,@Type,@Price);";

                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Type", product.Category);
                command.Parameters.AddWithValue("@Price", product.Price);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private Product RetrieveProducts(int id)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select Name, Type, Price from dbo.Products where id = @ID";
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        product = new Product
                        {
                            Name = reader[0].ToString(),
                            Category = reader[1].ToString(),
                            Price = (decimal)reader[2]
                        };

                        break;
                    }
                }

                connection.Close();
                return product;
            }
        }

        private List<Product> RetrieveAllProducts()
        {
            var productList = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlString = "select Name, Type, Price from dbo.Products";
                SqlCommand command = new SqlCommand(sqlString, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Name = reader[0].ToString(),
                            Category = reader[1].ToString(),
                            Price = (decimal)reader[2]
                        };

                        productList.Add(product);
                    }
                }

                connection.Close();
                return productList;
            }
        }

    }
}