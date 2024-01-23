using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;


namespace WhiteLagoon.Web.Controllers
{
    [Authorize] //only signed customers will be avialable to login
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var villaList = _unitOfWork.Villa.GetAll();
            return View(villaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if(obj.Name ==  obj.Description)
            {
                ModelState.AddModelError("", "Name and Description should not be the same");
            }
            if (!ModelState.IsValid)
            {
                TempData["error"]= "Errror! Villa can't be created";
                return View();
            }
            if (obj.VillaImage != null)
            {
                string saveImageName = DateTimeOffset.Now.ToString("s").Replace(":", "_") + obj.VillaImage.FileName.ToString();
                string saveDestination = _webHostEnvironment.WebRootPath + @"\img\VillaImages\";
                //writing data and on destination
                try
                {
                    using (FileStream fs = new FileStream(saveDestination + saveImageName, FileMode.Create))
                    {
                        obj.VillaImage.CopyTo(fs);
                    }   
                }catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //setting image url
                obj.ImageUrl = "/img/VillaImages/" + saveImageName;
            }
            else
            {
                obj.ImageUrl = @"https://placehold.co/600x400";
            }
            _unitOfWork.Villa.Add(obj);
            _unitOfWork.Villa.Save();
            //   ViewBag.success = "Villa has been sucessfully added";
            TempData["success"] = "Villa has been sucessfully added";
            return RedirectToAction("Index","Villa");
        }
        public IActionResult Update(int villaId)
        {
            Villa? villaObj = _unitOfWork.Villa.Get(villa => villa.Id == villaId);
            if(villaObj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaObj);
        }
        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("", "Name and Description should not be the same");
            }
            if (ModelState.IsValid && obj.Id != 0 )
            {
                if(obj.VillaImage != null)
                {
                    string updateImageFile = DateTimeOffset.Now.ToString("s").Replace(":", "_") + obj.VillaImage.FileName.ToString();
                    string saveDestination = _webHostEnvironment.WebRootPath + @"\img\VillaImages\";
                    
                    try
                    {
                        //Delete the Existing Image
                        if (!string.IsNullOrEmpty(obj.ImageUrl)){

                            string oldImageLocation = Path.GetFullPath(_webHostEnvironment.WebRootPath.ToString() + obj.ImageUrl.Replace("/","\\"));
                            
                            if (System.IO.File.Exists(oldImageLocation))
                            {
                                System.IO.File.Delete(oldImageLocation);
                            }
                        }
                        //Write the Updated Image
                        using (FileStream fs =new FileStream(saveDestination+updateImageFile, FileMode.Create))
                        {
                            obj.VillaImage.CopyTo(fs);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    //setting image url
                    obj.ImageUrl = "/img/VillaImages/" + updateImageFile;
                }
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Villa.Save();
                TempData["success"] = $"The {obj.Name} Villa is successfully updated";
                return RedirectToAction("Index", "Villa");
            }
            ViewBag.error = $"Error! Couldn't Update the {obj.Name} Villa";
             return View();
            
        }

        [HttpGet]
        public IActionResult Delete(int villaId)
        {
            Villa? villaObj = _unitOfWork.Villa.Get(villa => villa.Id == villaId);
            if (villaObj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaObj);
        }

        [HttpPost]
        public IActionResult DeleteVilla(int villaId)
        {
            try
            {
                Villa? selectedVilla = _unitOfWork.Villa.Get(villa=>villa.Id ==villaId);
                if (selectedVilla is not null)
                {
                        // Delete the file
                    try
                    {
                        string filePath = (_webHostEnvironment.WebRootPath + selectedVilla.ImageUrl).Replace("/", "\\");
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                    }
                    catch(Exception ex) 
                    { 
                        Console.WriteLine(ex.Message); 
                    }

                    _unitOfWork.Villa.Remove(selectedVilla);
                    _unitOfWork.Villa.Save();
                    TempData["success"] = "Villa has been deleted successfully.";
                    return RedirectToAction("Index", "Villa");
                }
                else
                {
                    // Villa not found, return a 404 Not Found response or handle it appropriately.
                    TempData["error"] = $"Villa with ID {villaId} not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error! Villa couldn't be deleted /n {ex.ToString()}";
                // Log the exception or handle it appropriately.
                return RedirectToAction("Index", "Villa");
            }
            return RedirectToAction("Index", "Villa");
        }

    }
}
