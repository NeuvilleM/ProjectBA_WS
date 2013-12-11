using JSONapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiV2.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<StageGroup> Get()
        {
            FestivalData fd = new FestivalData();
            fd.Stages = fd.GetFestivalData();        
            return fd.Stages;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}