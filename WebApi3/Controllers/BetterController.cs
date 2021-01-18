using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi3.Models;

namespace WebApi3.Controllers
{
    /// <summary>
    /// Agupa asociadas por distribuidora
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("betterware")]
    public class BetterController : ApiController
    {
        [HttpPost]
        [Route("agruparAsociadas")]
        public async Task<IHttpActionResult> agruparAsociadas()
        {
            var rawMessage = await Request.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawMessage);

            List<Distribuidor> Distribuidora = new List<Distribuidor> { };

            foreach (var data in jsonResponse)
            {
                var ASOC = data.Key;
                var DIST = data.Value;

                if (Distribuidora.Any(dis => dis.code == DIST))
                {
                    var dist = Distribuidora.Find(r => r.code == DIST);

                    if (!dist.asoc.Any(dis => dis.code == ASOC))
                    {
                        Asociado asociado = new Asociado() { code = ASOC };
                        dist.asoc.Add(asociado);
                    }
                }
                else
                {
                    List<Asociado> Asociada = new List<Asociado> { };
                    Asociada.Add(new Asociado() { code = ASOC });
                    Distribuidora.Add(new Distribuidor() { code = DIST, asoc = Asociada });
                }
            }

            return Ok(Distribuidora);
        }

        /// <summary>
        /// Personas vivas por año
        /// </summary>
        [HttpPost]
        [Route("inegi")]
        public async Task<IHttpActionResult> GetSomething()
        {
            var rawMessage = await Request.Content.ReadAsStringAsync();
            JObject rss = JObject.Parse(rawMessage);
            var jsonResponse = (JArray)rss["years"];

            List<Persona> persona = new List<Persona> { };
            foreach (var data in jsonResponse)
            {
                persona.Add(new Persona { nacimiento = int.Parse((string)data.First), muerte = int.Parse((string)data.Last) });
            }

            var añoInicial = persona.Min(x => x.nacimiento);
            var añofiinal = persona.Max(x => x.muerte);

            List<Años> año = new List<Años> { };
            for (int i = añoInicial; i <= añofiinal; i++)
            {
                año.Add(new Años() { año = i, noPersonasVivas = 0 });
            }

            foreach (var data in jsonResponse)
            {
                año.Where(x => x.año >= int.Parse((string)data.First) && x.año <= int.Parse((string)data.Last)).Select(c => { c.noPersonasVivas = c.noPersonasVivas + 1; return c; }).ToList();
            }

            var AñoMasPersonasVivas = año.OrderByDescending(x => x.noPersonasVivas).First();
            return Ok(AñoMasPersonasVivas);
        }
    }
}
