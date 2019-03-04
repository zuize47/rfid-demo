using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLE.Client.Data
{
    public interface IApiClient
    {
        List<KhoResultDTO> GetKhoResults();

        KhoDTO XuatKho(XuatkhoDTO dto);

        bool KiemOk(string rfid);

        bool ResetKiem();
    }
}
