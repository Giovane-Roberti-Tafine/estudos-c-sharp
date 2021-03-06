﻿using System;
using System.IO;
using System.Linq;

namespace Linq
{
    class Program
    {
        public static string[] obterTodosArquivosDotNet()
        {
            return Directory.GetFiles(
                    @"C:\Windows\Microsoft.NET\Framework\v4.0.30319",
                    "*",
                    SearchOption.AllDirectories
            );
        }

        static void Main(string[] args)
        {
            // Query onde sera listado todos os arquivos do diretorio
            // Sendo atribuido o caminho completo
            var query1 = from arquivo in Directory.GetFiles(@"c:\windows")
                         select arquivo;

            Console.WriteLine("query1 -------------------------");
            foreach (var arq in query1)
            {
                Console.WriteLine($"{arq}");

            }

            // Para apresentar somente o nome do arquivo, sem o caminho
            //e mais a informacao da extensao, pode-se fazer uma projecao.
            // O recurso dos tipos anonimos, permite fazer uma projeção do resultado
            //de uma consulta LINQ sem necessidade de se criar um novo tipo manualmente

            var query2 = from arquivo in Directory.GetFiles(@"C:\Windows")
                         select new
                         {
                             NomeArquivo = Path.GetFileName(arquivo),
                             Extensao = Path.GetExtension(arquivo)
                         };

            Console.WriteLine("query2 -------------------------");
            foreach (var arq in query2)
                Console.WriteLine($"{arq}");

            // Clásulas let, orderby, ascending e descending

            // Let permite introduzir uma variavel para armazenar resultados
            //de expressoes intermediarias numa expressao de consulta
            // Deste modo, o resultado armazenado na variavel pode ser reutilizado na consulta

            // Orderby, classifica os resultados em ordem ascendente, definido pelo uso
            //opcional da clausula ascending, ou descendente, definido pelo uso obrigatorio
            //da clausula descending

            // A clausula orderby pode ser usada para classificar o resultado da consulta
            //a seguir em ordem crescente de extensão (opcional o uso do ascending)
            //e decrescente de nome de arquivo (obrigatório o uso do descending)
            // A clausula let pode se usada para evitar a repiticao das operacoes de extracao
            //do nome de arquivo e da extensao dos nomes completos dos arquivos

            var query3 = from arquivos in Directory.GetFiles(@"C:/Windows")
                         let nomeArquivo = Path.GetFileName(arquivos)
                         let extensao = Path.GetExtension(arquivos).ToUpper()
                         orderby extensao, nomeArquivo descending
                         select new
                         {
                             NomeArquivo = nomeArquivo,
                             Extensao = extensao
                         };

            Console.WriteLine("query3 -------------------------");
            foreach (var arq in query3)
                Console.WriteLine(arq);


            // Cláusula where
            // A cláusula where permite filtrar elementos da fonte de dados baseada em um ou mais
            //expressôes booleanas separadas pelos operadores lógicos &&(e) ou ||(ou)

            // Por exemplo, se obter uma relação com todos os arquivos executáveis da pasta
            //C:/Windows com tamanho superior a 1mb, classificados em ordem crescente do tamanho do arquivo

            var query4 = from arquivo in Directory.GetFiles(@"C:/Windows")
                         let infoArquivo = new FileInfo(arquivo)
                         let tamanhoArquivoMB = infoArquivo.Length / 1024M / 1024M
                         where tamanhoArquivoMB > 1M &&
                            infoArquivo.Extension.ToUpper() == ".EXE"
                         orderby tamanhoArquivoMB
                         select new
                         {
                             Nome = infoArquivo.Name,
                             Tamanho = tamanhoArquivoMB
                         };

            Console.WriteLine("query4 -------------------------");
            foreach (var arq in query4)
                Console.WriteLine(arq);

            // Cláusula group, by e into
            // A cláusula group agrupa os resultados de uma consulta de acordo com valores específicos de chaves.
            //Ela é usada em conjunto com as palavras-chave by e into

            // A cláusula group retorna uma sequência de objetos do tipo System.Linq.IGrouping<TKey, TElement>,
            //que contém zero ou mais itens que correspondem ao valor da chave para o grupo

            // O LINQ fornece uma série de Standard Query Operators para executar operações de agregação em dados agrupados,
            //assim como ocorrem com o SQL. As operações de agregação disponíveis, estão expostas por meio de métodos de extensão
            //definidos nas classes estáticas System.Linq.Enumerable e System.Linq.Queryable

            var query5 = from arquivo in Directory.GetFiles(@"C:\Windows")
                         let infoArquivo = new FileInfo(arquivo)
                         group infoArquivo by infoArquivo.Extension.ToUpper() into g
                         let extensao = g.Key
                         orderby extensao
                         select new
                         {
                             Extensao = extensao,
                             NumeroArquivos = g.Count(),
                             TamanhoTotalArquivosKB = g.Sum(fi => fi.Length) / 1024M,
                             TamanhoMedioArquivoKB = g.Average(fi => fi.Length) / 1024D,
                             TamanhoMenorArquivosKB = g.Min(fi => fi.Length) / 1024D,
                             TamanhoMaiorArquivosKB = g.Max(fi => fi.Length) / 1024M
                         };

            Console.WriteLine("query5 -------------------------");
            foreach (var arq in query5)
                Console.WriteLine(arq);

            // Utilizando group by into com tipo anônimo e ordenamento

            var query6 = from arquivo in obterTodosArquivosDotNet()
                         let infoArquivo = new FileInfo(arquivo)
                         group infoArquivo by new
                         {
                             Pasta = infoArquivo.DirectoryName,
                             Extensao = infoArquivo.Extension.ToUpper()
                         }
                         into infoArquivosPorPastaExtensao
                         let tamanhoKB = infoArquivosPorPastaExtensao.Sum(i => i.Length) / 1024M
                         orderby infoArquivosPorPastaExtensao.Key.Pasta,
                            tamanhoKB descending
                         select new
                         {
                             infoArquivosPorPastaExtensao.Key.Pasta,
                             infoArquivosPorPastaExtensao.Key.Extensao,
                             NumeroArquivos = infoArquivosPorPastaExtensao.Count(),
                             tamanhoKB
                         };

            Console.WriteLine("query6 -------------------------");
            foreach (var arq in query6)
                Console.WriteLine(arq);



            // Cláusula join, in, on e equals
            // No código entre a cláusula "join" e a palavra-chave "in" há um identificador (tamanhoTotalPorPasta)
            //que representa cada elemento da fonte de dados (tamanhosTotaisPorPasta). Em seguida a palavra-chave "on"
            //identifca a condição de junção, com uso obrigatório da palavra-chave equals para realizar a comparação de igualdade

            // A junção é realizada pela comparação das pastas em ambas as coleções. Finalmente, o resultado da junção
            //é armazenado na variável cujo identificador é definido após a palavra-chave "into" (juncaoComTamanhoJB).
            // Neste caso, a junção sempre retorna um único elemento correspondente da fonte de dados tamanhosTotaisPorPasta,
            //porém, o resultado sempre é armazenado numa coleção, uma vez que a comparação poderia trazer vários elementos.
            // Sendo assim, foi usado o Standard Query Operator Single() para extrair o elemento único e acessar a sua propriedade
            //com o tamanho total em KB (tamanhoTotalKB)

            string[] arquivosDotNet = obterTodosArquivosDotNet();

            var tamanhosTotaisPorPasta = from arquivo in arquivosDotNet
                                         let infoArquivo = new FileInfo(arquivo)
                                         group infoArquivo by infoArquivo.DirectoryName into g
                                         select new
                                         {
                                             Pasta = g.Key,
                                             TamanhoTotalKB = g.Sum(i => i.Length) / 1024M
                                         };

            Console.WriteLine("query7 -------------------------");
            Console.WriteLine("tamanhosTotaisPorPasta -----------");
            foreach (var arq in tamanhosTotaisPorPasta)
                Console.WriteLine(arq);


            var query7 = from arquivo in arquivosDotNet
                         let infoArquivo = new FileInfo(arquivo)
                         group infoArquivo by new
                         {
                             Pasta = infoArquivo.DirectoryName,
                             Extensao = infoArquivo.Extension.ToUpper()
                         }
                        into infoArquivoPorPastaExtensao
                         //select new
                         //{
                         //    infoArquivoPorPastaExtensao.Key.Pasta,
                         //    infoArquivoPorPastaExtensao.Key.Extensao
                         //};
                         join tamanhoTotalPorPasta in tamanhosTotaisPorPasta
                         on infoArquivoPorPastaExtensao.Key.Pasta
                         equals tamanhoTotalPorPasta.Pasta
                         into juncaoComTamanhoTotalKB
                         //select new
                         //{
                         //    juncaoComTamanhoTotalKB.Single().Pasta,
                         //    juncaoComTamanhoTotalKB.Single().TamanhoTotalKB
                         //};
                         let tamanhoTotalKB = juncaoComTamanhoTotalKB.Single().TamanhoTotalKB
                         let tamanhoKB = infoArquivoPorPastaExtensao.Sum(i => i.Length) / 1024M
                         //select new
                         //{
                         //    tamanhoTotalKB,
                         //    tamanhoKB
                         //};
                         orderby infoArquivoPorPastaExtensao.Key.Pasta,
                            tamanhoKB descending
                         select new
                         {
                             infoArquivoPorPastaExtensao.Key.Pasta,
                             infoArquivoPorPastaExtensao.Key.Extensao,
                             NumeroArquivos = infoArquivoPorPastaExtensao.Count(),
                             Tamanho = tamanhoKB,
                             Porcetagem = 100 * (tamanhoKB / tamanhoTotalKB)
                         };


            Console.WriteLine("juncaoComTamanhoTotalKB -----------");
            foreach (var arq in query7)
                Console.WriteLine(arq);

           




           

            



        }
    }
}
