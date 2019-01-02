using CamDoAnhTu.Models;
using CamDoAnhTu.ViewModel;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace CamDoAnhTu.Controllers
{
    public class TimKiemNoKhachHangController : Controller
    {
        public ActionResult LoadData(DataTableAjaxPostModel model)
        {
            try
            {
                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    int filteredResultsCount;
                    int totalResultsCount;
                    var res = FilteredData(model, out filteredResultsCount, out totalResultsCount);

                    return Json(new
                    {
                        draw = model.draw,
                        recordsTotal = totalResultsCount,
                        recordsFiltered = filteredResultsCount,
                        data = res
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<TimKiemNoKhachHangVM> FilteredData(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                var searchBy = (model.search != null) ? model.search.value : null;
                var take = model.length;
                var skip = model.start;

                string sortBy = "";
                bool sortDir = true;

                if (model.order != null)
                {

                    sortBy = model.columns[model.order[0].column].data;
                    sortDir = model.order[0].dir.ToLower() == "asc";
                }

                var whereClause = BuildDynamicWhereClause(ctx, searchBy);

                if (String.IsNullOrEmpty(searchBy))
                {
                    sortBy = "Name";
                    sortDir = true;
                }

                var result = ctx.Customers
                               .Where(whereClause)

                               .Select(m => new TimKiemNoKhachHangVM
                               {
                                   Name = m.Name,
                                   Code = m.Code,
                                   Address = m.Address,
                                   Price = m.Price,
                                   Phone = m.Phone

                               })
                               .OrderBy(sortBy, sortDir)
                               .Skip(skip)
                               .Take(take);

                var abc  = (from p in result
                          where Convert.ToInt32(p.Code[p.Code.Length - 1]) % 2 == 0
                          select p).ToList();

                filteredResultsCount = ctx.Customers.AsExpandable().Where(whereClause).Count();
                totalResultsCount = ctx.Customers.Count();

                if (abc == null)
                {
                    return new List<TimKiemNoKhachHangVM>();
                }
                return abc;
            }
        }

        private Expression<Func<Customer, bool>> BuildDynamicWhereClause(CamdoAnhTuEntities1 entities, string searchValue)
        {

            var predicate = PredicateBuilder.New<Customer>(true); // true -where(true) return all
            
           
            if (String.IsNullOrWhiteSpace(searchValue) == false)
            {
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());


                predicate = predicate.Or(s => searchTerms.Any(srch => s.Code.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.Address.ToLower().Contains(srch)));
            }
            return predicate;
        }
    }
}