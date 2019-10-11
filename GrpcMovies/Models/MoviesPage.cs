using System.Collections.Generic;

namespace Models{
    public class MoviesPage{

        public int Page {get; set;} //pagina atual
        public int Per_page {get; set;}//quantos resultados por página
        public int Total {get; set;} //total de filmes
        public int Total_pages {get; set;} //total de páginas
        public List<MovieData> Data {get; set;}
    }
}