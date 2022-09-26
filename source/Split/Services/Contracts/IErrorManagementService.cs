using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Services
{
    public interface IErrorManagementService
    {
        void HandleError(Exception ex);
    }
}
