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

                var whereClause = BuildDynamicWhereClause(ctx, searchBy, model);

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
                                   Phone = m.Phone,
                                   NgayNo = m.NgayNo.Value,
                                   CodeSort = m.CodeSort.Value
                               })
                               .OrderBy(m => m.CodeSort) // sortby sortdir
                               .Skip(skip)
                               .Take(take);

                filteredResultsCount = ctx.Customers.AsExpandable().Where(whereClause).Count();
                totalResultsCount = ctx.Customers.Count();

                return result.ToList();
            }
        }

        private Expression<Func<Customer, bool>> BuildDynamicWhereClause(CamdoAnhTuEntities1 entities, string searchValue,
            DataTableAjaxPostModel model)
        {

            var predicate = PredicateBuilder.New<Customer>(true); // true -where(true) return all
            predicate = predicate.And(s => s.IsEven == model.iseven);
            predicate = predicate.And(s => s.type == model.type);

            if (Int32.TryParse(model.min,out int minVal))
            {
                predicate = predicate.And(s => s.NgayNo >= minVal);
            }

            if (Int32.TryParse(model.max, out int maxval))
            {
                predicate = predicate.And(s => s.NgayNo <= maxval);
            }

            if (String.IsNullOrWhiteSpace(searchValue) == false)
            {
                var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());

                predicate = predicate.Or(s => searchTerms.Any(srch => s.Code.ToLower().Contains(srch)));
                predicate = predicate.Or(s => searchTerms.Any(srch => s.Address.ToLower().Contains(srch)));
                //predicate = predicate.And(s => (Convert.ToInt32(s.Code.Remove(0, 1)) % 2) == 0);
               
            }
            return predicate;
        }
    }
}