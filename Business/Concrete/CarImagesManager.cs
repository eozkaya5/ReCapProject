﻿using Business.Abstract;
using Business.Contants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autfac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Helper;
using Core.Utilities.Result.Abstract;
using Core.Utilities.Result.Concrete;
using DataAccess.Abstract;

using Microsoft.AspNetCore.Http;
using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CarImagesManager : ICarImagesService
    {
        ICarImagesDal _carImageDAL;

        public CarImagesManager(ICarImagesDal carImageDAL)
        {
            _carImageDAL = carImageDAL;
        }
        [ValidationAspect(typeof(CarImagesValidator))]
        public IResult Add(IFormFile file, CarImage carImage)
        {
            IResult result = BusinessRules.Run(CheckImageLimitExceeded(carImage.CarId));
            if (result != null)
            {
                return result;
            }
            carImage.ImagePath = FileHelper.Add(file);
            carImage.Date = DateTime.Now;
            _carImageDAL.Add(carImage);
            return new SuccessResult();
        }

        [ValidationAspect(typeof(CarImagesValidator))]
        public IResult Delete(CarImage carImage)
        {
            FileHelper.Delete(carImage.ImagePath);
            _carImageDAL.Delete(carImage);
            return new SuccessResult();
        }


        [ValidationAspect(typeof(CarImagesValidator))]
        public IResult Update(IFormFile file, CarImage carImage)
        {
            IResult result = BusinessRules.Run(CheckImageLimitExceeded(carImage.CarId));
            if (result != null)
            {
                return result;
            }
            carImage.ImagePath = FileHelper.Update(_carImageDAL.Get(p => p.Id == carImage.Id).ImagePath, file);
            carImage.Date = DateTime.Now;
            _carImageDAL.Update(carImage);
            return new SuccessResult();
        }

        [ValidationAspect(typeof(CarImagesValidator))]
        public IDataResult<CarImage> Get(int id)
        {
            return new SuccessDataResult<CarImage>(_carImageDAL.Get(p => p.Id == id));
        }

        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDAL.GetAll());
        }

        [ValidationAspect(typeof(CarImagesValidator))]
        public IDataResult<List<CarImage>> GetImagesByCarId(int id)
        {
            return new SuccessDataResult<List<CarImage>>(CheckIfCarImageNull(id));
        }

        //business rules
        private IResult CheckImageLimitExceeded(int carid)
        {
            var carImagecount = _carImageDAL.GetAll(p => p.CarId == carid).Count;
            if (carImagecount >= 5)
            {
                return new ErrorResult(Messages.CarImageLimitExceeded);
            }

            return new SuccessResult();
        }
        private List<CarImage> CheckIfCarImageNull(int id)
        {
            string path = @"C: Users\Elif\Desktop\Projeler\Elif\Web Site\CarRentalProject\WebAPI\Images\logo.png";
            var result = _carImageDAL.GetAll(c => c.CarId == id).Any();
            if (!result)
            {
                return new List<CarImage> { new CarImage { CarId = id,ImagePath= path, Date = DateTime.Now } };
            }
            return _carImageDAL.GetAll(n => n.CarId == id);
        }

    }
}
