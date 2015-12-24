using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcWebApi;
using MvcWebApi.Models;

namespace ProductsApp.Controllers
{
    public class ProductsController : ApiController
    {
        Db _db;

        public ProductsController()
        {
            _db = Db.Instance;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _db.Products.GetEnumerable();
        }

        public IHttpActionResult GetProduct(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        public HttpResponseMessage PostProduct(Product item)
        {
            _db.Products.Add(item);
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new {id = item.Id});
            response.Headers.Location = new Uri(uri);
            return response;
        }


    }
}