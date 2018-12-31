using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using API.App_Start;
using API.Models;
using AutoMapper.QueryableExtensions;
using Backend.Database.Business;
using Backend.Database.Model;
using AutoMapper;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BasementController : ApiController
    {
        private readonly BasementRepository _basementRepository;

        public BasementController()
        {
            TaxiHeavenContext ctx = new TaxiHeavenContext();
            _basementRepository = new BasementRepository(ctx);
        }

        public IQueryable<BasementModel> Get()
        {
            return _basementRepository.Get().ProjectTo<BasementModel>();
        }
        [BasicHttpAuthorize(3)]
        public IHttpActionResult Post(BasementModel basement)
        {
            if (basement.Longitude == null || basement.Latitude == null || string.IsNullOrEmpty(basement.Name))
                return BadRequest("Wszystkie pola muszą być wypełnione.");
            var entity = Mapper.Map<Basement>(basement);
            _basementRepository.Post(entity);
            return Ok("Dodano bazę.");
        }
    }
}
