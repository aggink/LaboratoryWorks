using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.ViewModels
{
    public class NavMethodsForTablesViewModel
    {
        public List<NavMethodsForTablesViewModel<ParamSort>>? Sort { get; set; }
        public NavMethodsForTablesViewModel<ParamSearch>? Search { get; set; }
        public NavMethodsForTablesViewModel(List<NavMethodsForTablesViewModel<ParamSort>> sort, NavMethodsForTablesViewModel<ParamSearch> search)
        {
            Sort = sort;
            Search = search;
        }
    }

    public class NavMethodsForTablesViewModel<T> where T : class
    {
        public string Name { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public List<T> Param { get; set; } = null!;
    }

    public class ParamSort
    {
        public string Name { get; set; } = null!;
        public string ObjectSort { get; set; } = null!;
        public TypeSort TypeSort { get; set; }

        public ParamSort(string name, string objectSort, TypeSort typeSort)
        {
            Name = name;
            ObjectSort = objectSort;
            TypeSort = typeSort;
        }
    }

    public class ParamSearch
    {
        public string Name { get; set; } = null!;
        public string ObjectSearch { get; set; } = null!;
        public ParamSearch(string name, string objectSearch)
        {
            Name = name;
            ObjectSearch = objectSearch;
        }
    }

    public enum TypeSort
    {
        OrderBy,
        OrderByDescending
    }
}
