using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Models;

namespace GrpcMovies
{
    public class GreeterService : Greeter.GreeterBase
    {
        HttpClient client = new HttpClient();

        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public async Task<List<MovieData>> GetMoviebyNameAndPage(String name, String numberPage)
        {
            try
            {
                string url = "https://jsonmock.hackerrank.com/api/movies/search/?Title=" + name
                           + "&page=" + numberPage;

                var response = await client.GetStringAsync(url);

                MoviesPage moviesPage = JsonConvert.DeserializeObject<MoviesPage>(response);
                return moviesPage.Data; //return only name and years of movies
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao pesquisar o filme " + name + " na página " + numberPage.ToString() + "."
                      + "Excessão: " + ex.Message);
            }
        }

        public override async Task<Movies> GetMovieByName(NameMovie request, Grpc.Core.ServerCallContext context)
        {
            try
            {
                var httpContext = context.GetHttpContext();
                var clientCertificate = httpContext.Connection.ClientCertificate;

                string url = "https://jsonmock.hackerrank.com/api/movies/search/?Title="+ request.Name;
                var response = await client.GetStringAsync(url);

                MoviesPage moviesPage = JsonConvert.DeserializeObject<MoviesPage>(response);

                //Verificar de precisa inicializar.
                Movies movies = new Movies();
                movies.Total = moviesPage.Total;

                //Run the next iteration, because the first was executed
                for (int i = 2; i <= moviesPage.Total_pages; i++)
                {
                    List<MovieData> movieData = new List<MovieData>();
                    movieData = await GetMoviebyNameAndPage(request.Name, i.ToString());
                    moviesPage.Data.AddRange(movieData);
                }

                foreach (var line in moviesPage.Data.GroupBy(movie => movie.Year)
                        .Select(group => new {
                            Year = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Year))
                        {
                    MoviesByYear moviesByYear = new MoviesByYear();
                    moviesByYear.Year = line.Year;
                    moviesByYear.Movies = line.Count;
                    movies.MoviesByYear.Add(moviesByYear);
                }

                return movies;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao pesquisar o filme " + request.Name + "."
                      + "Excessão: " + ex.Message);
            }
        }
    }
}
