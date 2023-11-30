using jQueryAjaxInAsp.NETMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxInAsp.NETMVC.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View(GetAllEmployees());
        }
        //from this view we will return all of the Employees
        /*right click on this method to add view
         * View Name: ViewAll
         * Template:List
         * Model class: Employee(jQueryAjaxInAsp.NETMVC.Models)
         */
        //ViewAll row in navigation tab
       public ActionResult ViewAll()
        {
            return View(GetAllEmployees());
        }
        // this function will return IEnumerable collection of Employee
        //this works for read only access to a collection
        IEnumerable<Employee> GetAllEmployees()
        {
            using (DBModel db = new DBModel())
            {
                return db.Employees.ToList<Employee>();
            }

        }

        //Insert operation to add new employees
        //GET
        public ActionResult AddOrEdit(int id = 0)
        {
            Employee emp = new Employee();
            if (id != 0)
            {
                using (DBModel db = new DBModel())
                {
                    emp = db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                }
            }
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddOrEditEmployee(Employee emp)
        {
            try
            {
                if (emp.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.ImageUpload.FileName);
                    string extension = Path.GetExtension(emp.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    emp.ImagePath = "~/AppFiles/Images/" + fileName;
                    emp.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                using (DBModel db = new DBModel())
                {
                    if (emp.EmployeeID == 0)
                    {
                        db.Employees.Add(emp);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployees()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (DBModel db = new DBModel())
                {
                    Employee emp = db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployees()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}