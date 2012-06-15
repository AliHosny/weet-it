using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WcfServiceLibrary2
{
    [ServiceContract]
    public interface Interface1
    {
        [OperationContract]
        List<List<String>> CompareWithRespect(List<String> subjectsNames, String predicateURI, int limit);
    }
}
