﻿using Core.Utilities.Result.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
   public interface IUserService
    {
        IResult Add(User user);

        IResult Delete(User car);

        IResult Update(User car);
      
        IDataResult<List<User>> GetAll();
    }
}