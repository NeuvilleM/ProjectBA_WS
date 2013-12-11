using JSONapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JSONapi.Controllers
{
    public class FestivalDataController : ApiController
    {
        // GET api/festivaldata
        public IEnumerable<StageGroup> Get()
        {
            FestivalData fd = new FestivalData();
            fd.Stages = fd.GetFestivalData();
            return fd.Stages;
        }

        // GET api/festivaldata/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/festivaldata
        public void Post([FromBody]string value)
        {
        }

        // PUT api/festivaldata/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/festivaldata/5
        public void Delete(int id)
        {
        }
    }
}
