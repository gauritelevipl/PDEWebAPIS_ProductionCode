using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PDEWebAPIS.Services
{
    public class EPCISConfig
    {
        public string BearerToken { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
    }

    public class EPCISAPIService
    {
        private AppDBContextR context;
        private static readonly HttpClient client = new HttpClient();
        Decryptor decryptor = new Decryptor();
        private readonly string _bearerToken;
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly ILogger<EPCISAPIService> _logger;

        public EPCISAPIService(AppDBContextR context, IOptions<EPCISConfig> config , ILogger<EPCISAPIService> logger)
        {
            this.context = context;
            _bearerToken = config.Value.BearerToken;
            _apiKey = config.Value.ApiKey;
            _secretKey = config.Value.SecretKey;
            this._logger = logger;
        }

        public string DecryptDataSecurity()
        {
            //string str = "VHdpTmMrRUs0WEVFTUZYQ1M0NUc0dkxXWnNFWHEvZGRlRXpnZi9MajZ1eWtkR05DQ21SWTlCQmhXUVh6M0F3S09yd2htblhWRVYvM2NUT1d6ZmE1Y0JqS2RmT3pFMHV6S2FvS3VrbW1kcDFld1hGSGJkWHZ5YU0xOTlwSWRNb2diOUhxZHlxQ2gxRitQRDNheVBNL21uK1dtSHBjeU5mbitCQzZBRlNaVUZQRlJTbzd5WXJTTzBFQmhheDhIa0VwaGZvNTBxQ084d2RSUVZmZk44Mk1FTFFxeWtpWENiV1llV2dLc2VrUkh0dHpHaVB0ay9UMWJSMmFKWXBlb01jTHJESDNOOC90TXZnbnhZWWVHZ1F2a3R1UWJjaUdtbDRzaDhCektEKzVlSWxYK3E0eGY3SjFnUFd5a0syK2pjN3IxTklBM1RvSE1QZ1lrUEt0M2U3dUYzMXB4SW5aZjAxajNIL3ZiZGtkTktlN2dDTDVwS0NtWUJGdXNNdUF4R1BHSno1S3NXeTJIdTMrQUprS1gzUEFFZEpRLzZMbXdGek9FZWN3N2trRmRGSmhxZEJpS0M5aVIreWJEVEdxK1htYU96Nm16OFV0b0JGa3p0MjBxMlNIT3NXSXhlbjR2SjdNZXVkbnlJWnFURzdidytNekNFZlVsYVlsam55UjRKYzZMSldhTkhNaEdIK0dyOGw0Sk1pQ01oNmQ1NVpKNWZDcmNjdWNGWmpFQU8rbnFCY29mSE5UTllLMUdmUEVoR1BZOFZLc1NqL0JkaWtjWUZlRWdUQytXQk13aWl6VGMybWlJMkYvL1QxUDJwMTRUc2Y4UGYyYjFPY09SaTMrcFJlc2RJc1BxcjErYTRxQXpVWkljMUJ6QU5rZWxCeFAvMUZZclVxYzN6RVNVRmlCZTk0MXdScTExYllYdWhyUWlwMGtjdy90YUVZalU4SzVlRlBNOUs2Z3d1aDdiT0NhUXpMdWYzUlhKQ2pjbmlKRmxrSGVVdGlZeGkvdTFkUXUvT1Bpb1NYZTdvaENyY1Q2RzJDR2xydU5Ua29oT0h0cmJQV0hpaTkvTndLSnhPdmVBSEtYSFJVYXRVNzdERnRvUGVObktjbS8weDNyaTRLbEVlMUk0V0JGMjZyTDluMkhmejd2WWI1VzA5OUViRi9JNFNGbjZaODhwTG9jZzlJRmJHUGNxMEFTVEl1Y1U5aElIQ0xJbGVXaE9WOWhFQjJKYitjK1d3NTgvN0o0TVo5dHVjRTUxdFlGRXk3Rkc2akIrUzI0YUJEYlZvT2xFR1ZKZCs3WldJSDFLK3N5dVBDR3hHcmZZY3pOT1pTQUpzUHkwcE1LMkV3LzQvbEtoZWxxVXR1djc4Vmc0YlQ2ZW9LQ1B2d0pMb0kzMWk2aEh2bUZ6K2tlYnBsY0lNcFAyR2VDVldZdnBSbDl0YzB0a2ZPKy9tYVZGdGZwRWdHNlZ2QWdOeS9nUTBBdk1hTHNWc05yaG5WbWNFbHpJcHI1YUV2V1hHUEdOM0dCWGhRS2t1SDlZcGhjWGRPbjVpcTF0RE9wUUo1UEFOMTZSdEo4MGtKMTVhQVltaVZpRldPRk5kdVUxbUQwYmpvMkd2cXRyaExqWDRYVEFYV2lDaVhEd1ZQblNKYkJHaWlvYzE5bU42YjhzOVlZa1dwY2IwWHR3eG9zZzVNbXoxSGxiTWZpcFR6RDdUQ2YvOEFRVC9UZVZEaTBxaHpzQU9pMGZYNHhDK0ZLdzVPeXh1NWEwYkNvYVZ3UWs2YzMvU05od3pqTXNVd1dXTGdyYkRMR1VINHF2TjhlblV3RURCMDhpM2RRQjVGaHJPditBaXlUd2lpL1Jxa0JpVHVISFB6UE0xanJoaTRkLzFKNS9qd2JvZVc2VEdCVGU5U0gzQUU0bnZmUUFxQjhSZ21LZmZramtXcjZHWU1QR2w0UlV1TTJucjZpSGsxSStxVUE3SGpkVW5JK0JjYXEvYTBtMGhyUjNNaGRiZys1dEZNR3FEUjBwWlB1b3JBb3phKzJaNm1RdG5QR21VSEp4SWdFaGZGWXZBL3gzRlZWdlRuUldOMjljeE9QRUI3VG9rOWRHaElORWtQcDlNcHBzZ0JkTWcxRnFJZURIRERWalBLWXdFcFRpWkF0VThYQ2hXeEJSSW1KdDBQeEFWdE5Ic08yNVgxMWpGYjdqOStkWHNNVFhQdzhBMUpIdmVYVStuRktMTDVBakJRZVVvbVp3clRLa2oxMElBQkFOQStIdzlkQVVNZ21JUkV4NU84VmVROXRVSlpVdDVLeU1qWUcwcXMwNDRmck0vK2hjS2pSVkJhQVhyeHQ4b3VYaGVOWFpDVmNPQUVhSVpnaHFnOTM1RisxVDlIMEV0VWpPUHJ0SUMxN2xhWml1eW81MUFzSytpNHJRRFFZcDdMYk5sVUhaSXYwblFIZW5kaHArUFpBRXRFS3VaVkNPb0phMVNVSjY2N2RhS21MTWdzWGdmV1N3bURZc2lpTHNRTkpicnk3cFB1RmlNSFpCdkE4TWdpNm0xd1BKVk9YUFowenVIOTRtK25QV0huYld1Ny8rK3JQVWp0UGVOOVc3Vm9hOEd1UTV6WDRDS1hBa2ZxUFVlcTIxbElPc3ZtRXRzNExZRytrNmFRV0xFY0tnWHJSVVMwS3MzTzFSenNNbmxNWTJHdk1XNU5Sc0lsQ2l0cXpXRGZkVW1GbDFtUWZBS0dKV3pBd1d5SWs3WlAwSTU1MFZsYmNXajY3WlJzM042NXhlYllaSVJWdGdwbC9haEYwMFNEc3VvVEN3cjkvdVJ2ckgvTStyVCtlRFQ3cjNVRk9LVG5xMHFPczVScERtKzZNOFN4eDRLcWhxZHJZc2NkMHpPckN3R04yMVNJR1NjTXBaSDBzZW5MRkdwL0FWK0ZjZkplRW5CRXFLekliMVE2c01YNzg5WXU2bXdoZ2QxdGUrY25nd3dNbDdlcjBiWHA1c0QzY1NldktmK1hDeVlnOEQ0N0hTcC9hL2wzclZWaVprTDVIWmRZVkM1djRxR0VWSE1BWDEzaGlpejVQWkV2R1NrQThSSThZMWNFRVVzVGdjYmZGVU5Xd0tGWG16OTJaZWhTb3FTbkxTcVYxRDY0bU92Wko4VEV1R2RnWUh4dWo4TENWcnI1c2ZiZWVZUkhrcllnRnR2c2hpbjRPQldZekNYazErQ3FNejNvaHhkQ0lvQUpVNnZIL1hMTFRFMlFlRjFaZFFET0NvUmoyUGdzRURaOG4vbCttVkc3ZTRWZFdDQUNKR3RxU1dCTGh0VGp2Q1BPNENBYUQ2SmVzT3RBblUyendlcHhPVXFaYzJsWjhxQ0tZWHo1QjNHYnJpSHVjQmFIdVliRFdNa1I5MUk0ZjlkNTVlYzhyemdYZnEzdWRiREhhZHlqR2JiYjVoQW5DQVc0Z095RHozSkhRZS85RGNWamdGeTJxeU9KMDhRb0l3VGtFRlNLRzhTWlNJVW1JeGN5dXBnVWx5b3pMNys4RXBCRXoyWHlLb0Zxb0p2VktqcmdvMEJSZ0FXQktKWmhhT3FCVUhpK1AvUG9zRUYrR3FwbS8yUngzVWkzd2V2YzlYRGo2aHgzNUF0cldQQ2xHN25lU1I5dncvTnpReGRnKzRxWG81WlVqZ2pLeXN6UzRVWVorQXYyWFBGYVpXWGszQ2lGK3J5S2xiMlhENm5VdWZtenM0dUlFcXFnTnlrT3JTU01KSzNoZG43ZUNVTTFtTEx2NGJYNWFJS1VaSnlSZC80L1lZWEIySFdGZHEva1JZTzIveGtSL1QrRWNHRHdjZkYxUlFtNzYyQWo4VjhoeVNZR1Raemx5N1dWb0J2Y2VRWlZLU3JiTU8xYmZjQjMwaUk0Y25sNGJoTUVtazdyRitXNkgwcnVhRGh6d1F0c05Zc3E5UmVmdzcrSWNIVk9yekVqKzUxVWtMb2FYSVFYaEwvd2tDNjNNMXhvUFYzL1E5a3lhR2F3cHByanZnRmFSK3JlTUExZXJLVGNNcDh6dmp0bDJ2OGpxWWJwSCtweTB2bEhSeWl2b0hZTWg1TTBYaGlIMHBmK2NxTlhOYmtmNXErRENwUDcvY04ranQ1dE1XTE8yZG12M1dndkRTd3lMYXptdUtYVm0xYkVuYTg5QU54Vm1tdkpiY2d6cHVRU1JXSC8wWWxUM1IxKzdnZXlURlhXNiszalJqWUk1eVF1aEROcFdsUGFrVDVNZ2Z6aWphN0VqQnhUYkR5T2FXbXR1ZkFGOWJhVnhuUmtxOFdKbjAwYWhCQlNQbHhRT1Z6N0pTbStjRWI0VUpKUGJielRRdWhHRjM2TDNMQmdBOG0vRWVLVGFwYjFDN3c3MmVBZTFlMTdtSVd6ZHp4SVBpeWRGeHBhSDBwbXRkWUhzQjBQZmxkUGUyOEw2NlYrRGg5SEpqcXJ6RTNiWVQ2c0wzdCt0MEZBMmxINkdNWFd3SGFXaVV1VFZ1RGVldTc2MDMySGVlOTZxZ0RuZ0FRV1lVaHd2TVJ5OHJvdDZBK2RQRWllT3U3Z1JOL3BDZlRHYVBEQTNTL3ZyZS8xNlIrYzdTazVQRmxjY3FFNVFtZ0R2b1Y3K3VGaVk5anZvU2NnQktNY01STDdUMUNNdGQrMGZJL0JFV1E0bW1pemEweGVpcFNRNlRxeS95Z3JYdzM1dnNQb0Q1WWhNRVdUVHdzRWl5SjVINFJ1TjJFb1pyU3Frcmp0ZG1OWGI2Kzl5d09yY0ZzYngxTllVWmtQZHRIZEVNSFFnczVkSEtkcTlUNHFVdkF2cmp2YjQxR2hsN2k5NmNnaXg1QmFpTXhGdGNZRDgyak9yRkVvYWM1QzV2WUJZL2RSYjN0dFo2L1pyU2Exa3l1MUVqcGEvVlZhaGk4ZzFITGwvSXFNc0V4Z2M3elJmYkZBc1l1N3IwVGcrb3hWN1EwTjNkU0g3QUl2QmwzS2xOM2xBMmM0Vkt3bmlDM0szYVdtUFZDUVViQytpVFFMdWpiam9UckJCUWpzbUVWY2RRd0g5MzEyOEU3VytJdUovM1FrWld6U2c1N3ROb2xWQkd3PT0=";
            string responsebody = "{\"status\":200,\"data\":\"VHdpTmMrRUs0WEVFTUZYQ1M0NUc0dkxXWnNFWHEvZGRlRXpnZi9MajZ1eWtkR05DQ21SWTlCQmhXUVh6M0F3S09yd2htblhWRVYvM2NUT1d6ZmE1Y0JqS2RmT3pFMHV6S2FvS3VrbW1kcDFld1hGSGJkWHZ5YU0xOTlwSWRNb2diOUhxZHlxQ2gxRitQRDNheVBNL21uK1dtSHBjeU5mbitCQzZBRlNaVUZQRlJTbzd5WXJTTzBFQmhheDhIa0VwaGZvNTBxQ084d2RSUVZmZk44Mk1FTFFxeWtpWENiV1llV2dLc2VrUkh0dHpHaVB0ay9UMWJSMmFKWXBlb01jTHJESDNOOC90TXZnbnhZWWVHZ1F2a3R1UWJjaUdtbDRzaDhCektEKzVlSWxYK3E0eGY3SjFnUFd5a0syK2pjN3IxTklBM1RvSE1QZ1lrUEt0M2U3dUYzMXB4SW5aZjAxajNIL3ZiZGtkTktlN2dDTDVwS0NtWUJGdXNNdUF4R1BHSno1S3NXeTJIdTMrQUprS1gzUEFFZEpRLzZMbXdGek9FZWN3N2trRmRGSmhxZEJpS0M5aVIreWJEVEdxK1htYU96Nm16OFV0b0JGa3p0MjBxMlNIT3NXSXhlbjR2SjdNZXVkbnlJWnFURzdidytNekNFZlVsYVlsam55UjRKYzZMSldhTkhNaEdIK0dyOGw0Sk1pQ01oNmQ1NVpKNWZDcmNjdWNGWmpFQU8rbnFCY29mSE5UTllLMUdmUEVoR1BZOFZLc1NqL0JkaWtjWUZlRWdUQytXQk13aWl6VGMybWlJMkYvL1QxUDJwMTRUc2Y4UGYyYjFPY09SaTMrcFJlc2RJc1BxcjErYTRxQXpVWkljMUJ6QU5rZWxCeFAvMUZZclVxYzN6RVNVRmlCZTk0MXdScTExYllYdWhyUWlwMGtjdy90YUVZalU4SzVlRlBNOUs2Z3d1aDdiT0NhUXpMdWYzUlhKQ2pjbmlKRmxrSGVVdGlZeGkvdTFkUXUvT1Bpb1NYZTdvaENyY1Q2RzJDR2xydU5Ua29oT0h0cmJQV0hpaTkvTndLSnhPdmVBSEtYSFJVYXRVNzdERnRvUGVObktjbS8weDNyaTRLbEVlMUk0V0JGMjZyTDluMkhmejd2WWI1VzA5OUViRi9JNFNGbjZaODhwTG9jZzlJRmJHUGNxMEFTVEl1Y1U5aElIQ0xJbGVXaE9WOWhFQjJKYitjK1d3NTgvN0o0TVo5dHVjRTUxdFlGRXk3Rkc2akIrUzI0YUJEYlZvT2xFR1ZKZCs3WldJSDFLK3N5dVBDR3hHcmZZY3pOT1pTQUpzUHkwcE1LMkV3LzQvbEtoZWxxVXR1djc4Vmc0YlQ2ZW9LQ1B2d0pMb0kzMWk2aEh2bUZ6K2tlYnBsY0lNcFAyR2VDVldZdnBSbDl0YzB0a2ZPKy9tYVZGdGZwRWdHNlZ2QWdOeS9nUTBBdk1hTHNWc05yaG5WbWNFbHpJcHI1YUV2V1hHUEdOM0dCWGhRS2t1SDlZcGhjWGRPbjVpcTF0RE9wUUo1UEFOMTZSdEo4MGtKMTVhQVltaVZpRldPRk5kdVUxbUQwYmpvMkd2cXRyaExqWDRYVEFYV2lDaVhEd1ZQblNKYkJHaWlvYzE5bU42YjhzOVlZa1dwY2IwWHR3eG9zZzVNbXoxSGxiTWZpcFR6RDdUQ2YvOEFRVC9UZVZEaTBxaHpzQU9pMGZYNHhDK0ZLdzVPeXh1NWEwYkNvYVZ3UWs2YzMvU05od3pqTXNVd1dXTGdyYkRMR1VINHF2TjhlblV3RURCMDhpM2RRQjVGaHJPditBaXlUd2lpL1Jxa0JpVHVISFB6UE0xanJoaTRkLzFKNS9qd2JvZVc2VEdCVGU5U0gzQUU0bnZmUUFxQjhSZ21LZmZramtXcjZHWU1QR2w0UlV1TTJucjZpSGsxSStxVUE3SGpkVW5JK0JjYXEvYTBtMGhyUjNNaGRiZys1dEZNR3FEUjBwWlB1b3JBb3phKzJaNm1RdG5QR21VSEp4SWdFaGZGWXZBL3gzRlZWdlRuUldOMjljeE9QRUI3VG9rOWRHaElORWtQcDlNcHBzZ0JkTWcxRnFJZURIRERWalBLWXdFcFRpWkF0VThYQ2hXeEJSSW1KdDBQeEFWdE5Ic08yNVgxMWpGYjdqOStkWHNNVFhQdzhBMUpIdmVYVStuRktMTDVBakJRZVVvbVp3clRLa2oxMElBQkFOQStIdzlkQVVNZ21JUkV4NU84VmVROXRVSlpVdDVLeU1qWUcwcXMwNDRmck0vK2hjS2pSVkJhQVhyeHQ4b3VYaGVOWFpDVmNPQUVhSVpnaHFnOTM1RisxVDlIMEV0VWpPUHJ0SUMxN2xhWml1eW81MUFzSytpNHJRRFFZcDdMYk5sVUhaSXYwblFIZW5kaHArUFpBRXRFS3VaVkNPb0phMVNVSjY2N2RhS21MTWdzWGdmV1N3bURZc2lpTHNRTkpicnk3cFB1RmlNSFpCdkE4TWdpNm0xd1BKVk9YUFowenVIOTRtK25QV0huYld1Ny8rK3JQVWp0UGVOOVc3Vm9hOEd1UTV6WDRDS1hBa2ZxUFVlcTIxbElPc3ZtRXRzNExZRytrNmFRV0xFY0tnWHJSVVMwS3MzTzFSenNNbmxNWTJHdk1XNU5Sc0lsQ2l0cXpXRGZkVW1GbDFtUWZBS0dKV3pBd1d5SWs3WlAwSTU1MFZsYmNXajY3WlJzM042NXhlYllaSVJWdGdwbC9haEYwMFNEc3VvVEN3cjkvdVJ2ckgvTStyVCtlRFQ3cjNVRk9LVG5xMHFPczVScERtKzZNOFN4eDRLcWhxZHJZc2NkMHpPckN3R04yMVNJR1NjTXBaSDBzZW5MRkdwL0FWK0ZjZkplRW5CRXFLekliMVE2c01YNzg5WXU2bXdoZ2QxdGUrY25nd3dNbDdlcjBiWHA1c0QzY1NldktmK1hDeVlnOEQ0N0hTcC9hL2wzclZWaVprTDVIWmRZVkM1djRxR0VWSE1BWDEzaGlpejVQWkV2R1NrQThSSThZMWNFRVVzVGdjYmZGVU5Xd0tGWG16OTJaZWhTb3FTbkxTcVYxRDY0bU92Wko4VEV1R2RnWUh4dWo4TENWcnI1c2ZiZWVZUkhrcllnRnR2c2hpbjRPQldZekNYazErQ3FNejNvaHhkQ0lvQUpVNnZIL1hMTFRFMlFlRjFaZFFET0NvUmoyUGdzRURaOG4vbCttVkc3ZTRWZFdDQUNKR3RxU1dCTGh0VGp2Q1BPNENBYUQ2SmVzT3RBblUyendlcHhPVXFaYzJsWjhxQ0tZWHo1QjNHYnJpSHVjQmFIdVliRFdNa1I5MUk0ZjlkNTVlYzhyemdYZnEzdWRiREhhZHlqR2JiYjVoQW5DQVc0Z095RHozSkhRZS85RGNWamdGeTJxeU9KMDhRb0l3VGtFRlNLRzhTWlNJVW1JeGN5dXBnVWx5b3pMNys4RXBCRXoyWHlLb0Zxb0p2VktqcmdvMEJSZ0FXQktKWmhhT3FCVUhpK1AvUG9zRUYrR3FwbS8yUngzVWkzd2V2YzlYRGo2aHgzNUF0cldQQ2xHN25lU1I5dncvTnpReGRnKzRxWG81WlVqZ2pLeXN6UzRVWVorQXYyWFBGYVpXWGszQ2lGK3J5S2xiMlhENm5VdWZtenM0dUlFcXFnTnlrT3JTU01KSzNoZG43ZUNVTTFtTEx2NGJYNWFJS1VaSnlSZC80L1lZWEIySFdGZHEva1JZTzIveGtSL1QrRWNHRHdjZkYxUlFtNzYyQWo4VjhoeVNZR1Raemx5N1dWb0J2Y2VRWlZLU3JiTU8xYmZjQjMwaUk0Y25sNGJoTUVtazdyRitXNkgwcnVhRGh6d1F0c05Zc3E5UmVmdzcrSWNIVk9yekVqKzUxVWtMb2FYSVFYaEwvd2tDNjNNMXhvUFYzL1E5a3lhR2F3cHByanZnRmFSK3JlTUExZXJLVGNNcDh6dmp0bDJ2OGpxWWJwSCtweTB2bEhSeWl2b0hZTWg1TTBYaGlIMHBmK2NxTlhOYmtmNXErRENwUDcvY04ranQ1dE1XTE8yZG12M1dndkRTd3lMYXptdUtYVm0xYkVuYTg5QU54Vm1tdkpiY2d6cHVRU1JXSC8wWWxUM1IxKzdnZXlURlhXNiszalJqWUk1eVF1aEROcFdsUGFrVDVNZ2Z6aWphN0VqQnhUYkR5T2FXbXR1ZkFGOWJhVnhuUmtxOFdKbjAwYWhCQlNQbHhRT1Z6N0pTbStjRWI0VUpKUGJielRRdWhHRjM2TDNMQmdBOG0vRWVLVGFwYjFDN3c3MmVBZTFlMTdtSVd6ZHp4SVBpeWRGeHBhSDBwbXRkWUhzQjBQZmxkUGUyOEw2NlYrRGg5SEpqcXJ6RTNiWVQ2c0wzdCt0MEZBMmxINkdNWFd3SGFXaVV1VFZ1RGVldTc2MDMySGVlOTZxZ0RuZ0FRV1lVaHd2TVJ5OHJvdDZBK2RQRWllT3U3Z1JOL3BDZlRHYVBEQTNTL3ZyZS8xNlIrYzdTazVQRmxjY3FFNVFtZ0R2b1Y3K3VGaVk5anZvU2NnQktNY01STDdUMUNNdGQrMGZJL0JFV1E0bW1pemEweGVpcFNRNlRxeS95Z3JYdzM1dnNQb0Q1WWhNRVdUVHdzRWl5SjVINFJ1TjJFb1pyU3Frcmp0ZG1OWGI2Kzl5d09yY0ZzYngxTllVWmtQZHRIZEVNSFFnczVkSEtkcTlUNHFVdkF2cmp2YjQxR2hsN2k5NmNnaXg1QmFpTXhGdGNZRDgyak9yRkVvYWM1QzV2WUJZL2RSYjN0dFo2L1pyU2Exa3l1MUVqcGEvVlZhaGk4ZzFITGwvSXFNc0V4Z2M3elJmYkZBc1l1N3IwVGcrb3hWN1EwTjNkU0g3QUl2QmwzS2xOM2xBMmM0Vkt3bmlDM0szYVdtUFZDUVViQytpVFFMdWpiam9UckJCUWpzbUVWY2RRd0g5MzEyOEU3VytJdUovM1FrWld6U2c1N3ROb2xWQkd3PT0=\"}";
            var jsonobject = JsonConvert.DeserializeObject<LgdApiResponse>(responsebody);
            var decrypted = decryptor.DecryptData(jsonobject!.Data!.ToString()!);
            return decrypted;
        }
        /*public async Task<string> SendRequestAsync(string urlmethod,HttpMethod method, ILogger _logger, object? body = null)
        {

            string url = "https://api.mahabhumi.gov.in/api/epcis/"+urlmethod;
            *//*string bearerToken = "";
            string apiKey = "";
            string secretKey = "";*//*
            // Set up headers
            //Production
            //bearerToken = "0uXCpuv3E1ZDnJRDO7xJiQhHuaM8PjYC71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa";
            //apiKey = "e06ae416-d3db-4b7e-8cbd-edc73f6c706a";
            //secretKey = "ObvLNRgvtbIYe2IC9t3IpzEmclzKEjkqHdjM6iP7ggN76zSoWP9M1pekkdYDEb7c";

            //Local
            *//*string bearerToken = "0uXCpuv3E1ZDnJRDO7xJiQhHuaM8PjYC71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa";//"URqyR0fRlC3B9dxaAlTR1Ra31QKZ9HHnVaTPMihixMlbvKnhCJPAtQ3qYPnCKbIB";
            string apiKey = "f3c040ae-4264-f1d1-ac58-486e2453";
            string secretKey = "9z3g7YaHCzwj4diHacM2Cdt8Cg1FOYVLjh2nOtRjGBz67Ygh3UiYzwcOe5By";*//*


            // Create a new HttpRequestMessage
            var request = new HttpRequestMessage(method, url);

            // Add body if provided
            if (body != null)
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            // Add headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            request.Headers.Add("API-KEY", _apiKey);
            request.Headers.Add("SECRET-KEY", _secretKey);
            
            // Send the request
            HttpResponseMessage response = await client.SendAsync(request);
            _logger.LogInformation("Send Request Url ePICS  - " + url);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseBody);
                _logger.Log(logLevel:LogLevel.Warning,"Send Response Async ePICS - "+ responseBody);
                var jsonobject = JsonConvert.DeserializeObject<LgdApiResponse>(responseBody);
                if (jsonobject!.Status == 200)
                {
                    
                        var decrypted = decryptor.DecryptData(jsonobject!.Data!);
                        return decrypted + "|" + (int)response.StatusCode;
                    
                }
               // var districts = JsonConvert.DeserializeObject<List<LGDDistrict>>(decrypted);
                else
                {
                    return "Data Not Found" +"|"+ "400";
                }
                //return jsonobject!.Data + "~" + (int)response.StatusCode;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent + "|" + (int)response.StatusCode;
            }
            // Read and return the response
            *//* string responseContent = await response.Content.ReadAsStringAsync();
             return (responseContent, (int)response.StatusCode);*//*
        }*/
        //public async Task<string> SendRequestAsync(string urlMethod, HttpMethod method, ILogger _logger, object? body = null)
        //{
        //    string url = "https://api.mahabhumi.gov.in/api/epcis/" + urlMethod;
        //    int maxRetries = 3;
        //    int delayMilliseconds = 1000;
        //    int attempt = 0;

        //    while (attempt < maxRetries)
        //    {
        //        attempt++;
        //        try
        //        {
        //            // Create a new HttpRequestMessage
        //            var request = new HttpRequestMessage(method, url);

        //            // Add body if provided
        //            if (body != null)
        //            {
        //                string jsonBody = JsonConvert.SerializeObject(body);
        //                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        //            }

        //            // Add headers
        //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        //            request.Headers.Add("API-KEY", _apiKey);
        //            request.Headers.Add("SECRET-KEY", _secretKey);

        //            // Send the request
        //            HttpResponseMessage response = await client.SendAsync(request);
        //            _logger.LogInformation($"Attempt {attempt}: Send Request URL ePICS - {url}");

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                if (!string.IsNullOrEmpty(responseBody))
        //                {
        //                    _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async ePICS - {responseBody}");

        //                    var jsonObject = JsonConvert.DeserializeObject<LgdApiResponse>(responseBody);
        //                    if (jsonObject != null && jsonObject.Status == 200)
        //                    {
        //                        if (jsonObject.Data is string encryptedData && !string.IsNullOrEmpty(encryptedData))
        //                        {
        //                            // Decrypt the data if it's a non-empty string
        //                            var decrypted = decryptor.DecryptData(encryptedData);
        //                            if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
        //                            {
        //                                _logger.LogInformation("Gauri Tele -> " + responseBody);
        //                                _logger.LogInformation("Gauri Tele D -> " + decrypted);
        //                                //_logger.LogInformation("body check in get villege by office: " + body);
        //                                return decrypted + "$" + (int)response.StatusCode;
        //                            }
        //                            else
        //                            {
        //                                return decrypted + "|" + (int)response.StatusCode;
        //                            }
        //                            //return decrypted + "|" + (int)response.StatusCode;
        //                        }
        //                        else if (jsonObject.Data is List<string>)
        //                        {
        //                            // Handle the case where data is an empty array
        //                            return "Data Not Found" + "|" + "400";
        //                        }
        //                        return "Data Not Found" + "|" + "400";
        //                    }
        //                    else if (jsonObject!.Status == 500)
        //                    {
        //                        _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 - {responseBody}");
        //                        return "Data Not Found" + "|" + "400";
        //                    }
        //                    else
        //                    {
        //                        return "Invalid Response" + "|" + "400";
        //                    }
        //                }
        //            }
        //            else if ((int)response.StatusCode == 500)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 - {responseBody}");
        //                return "Data Not Found" + "|" + "400";
        //            }
        //            else
        //            {
        //                _logger.LogError($"Attempt {attempt}: Error - {response.StatusCode}");
        //                string responseContent = await response.Content.ReadAsStringAsync();
        //                return $"Error {response.StatusCode}: {responseContent}";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Attempt {attempt}: Exception occurred - {ex.Message}");
        //        }

        //        if (attempt < maxRetries)
        //        {
        //            _logger.LogWarning($"Retrying... Attempt {attempt + 1} in {delayMilliseconds}ms");
        //            await Task.Delay(delayMilliseconds); // Wait before retrying
        //        }
        //    }

        //    _logger.LogError("All retry attempts failed.");
        //    return "Data Not Found" + "|" + "400";
        //}

        //new
        public async Task<string> SendRequestAsync(string urlMethod, HttpMethod method, ILogger _logger, object? body = null)
        {
            string url = "https://api.mahabhumi.gov.in/api/epcis/" + urlMethod;
            int maxRetries = 3;
            int delayMilliseconds = 1000;
            int attempt = 0;

            while (attempt < maxRetries)
            {
                attempt++;
                try
                {
                    // Create a new HttpRequestMessage
                    var request = new HttpRequestMessage(method, url);

                    // Add body if provided
                    if (body != null)
                    {
                        string jsonBody = JsonConvert.SerializeObject(body);
                        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    }

                    // Add headers
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
                    request.Headers.Add("API-KEY", _apiKey);
                    request.Headers.Add("SECRET-KEY", _secretKey);

                    // Send the request
                    HttpResponseMessage response = await client.SendAsync(request);
                    _logger.LogInformation($"Attempt {attempt}: Send Request URL ePICS - {url}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async ePICS - {responseBody}");

                            var jsonObject = JsonConvert.DeserializeObject<LgdApiResponse>(responseBody);
                            if (jsonObject != null && jsonObject.Status == 200)
                            {
                                if (jsonObject.Data is string encryptedData && !string.IsNullOrEmpty(encryptedData))
                                {
                                    // Decrypt the data if it's a non-empty string
                                    var decrypted = decryptor.DecryptData(encryptedData);
                                    if (urlMethod == "getVillageByOffice")
                                    {
                                        _logger.LogInformation("Gauri Tele -> " + responseBody);
                                        _logger.LogInformation("Gauri Tele D -> " + decrypted);
                                        //_logger.LogInformation("body check in get villege by office: " + body);
                                        return decrypted + "$VIPL" + (int)response.StatusCode;
                                    }
                                    else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                                    {
                                        return decrypted + "$" + (int)response.StatusCode;
                                    }
                                    else
                                    {
                                        return decrypted + "|" + (int)response.StatusCode;
                                    }
                                    //return decrypted + "|" + (int)response.StatusCode;
                                }
                                else if (jsonObject.Data is List<string>)
                                {
                                    // Handle the case where data is an empty array
                                    if (urlMethod == "getVillageByOffice")
                                    {
                                        return "Data Not Found" + "$VIPL" + "400";
                                    }
                                    else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                                    {
                                        return "Data Not Found" + "$" + "400";
                                    }
                                    else
                                    {
                                        return "Data Not Found" + "|" + "400";
                                    }
                                }
                                //return "Data Not Found" + "|" + "400";
                            }
                            //if we add $ for split then should use below code 
                            else if (jsonObject!.Status == 400)
                            {
                                if (urlMethod == "getVillageByOffice")
                                {
                                    return jsonObject!.message! + "$VIPL" + jsonObject!.Status;
                                }
                                else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                                {
                                    return jsonObject!.message! + "$" + jsonObject!.Status;
                                }
                                else if (urlMethod == "getOwnerNameInfo")
                                {
                                    return jsonObject!.message! + "|" + jsonObject!.Status;
                                }
                                else
                                {
                                    return jsonObject!.message! + "|" + jsonObject!.Status;
                                }
                            }

                            else if (jsonObject!.Status == 500)
                            {
                                _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 - {responseBody}");

                                if (urlMethod == "getVillageByOffice")
                                {
                                    return "Data Not Found" + "$VIPL" + "400";
                                }
                                if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                                {
                                    return "Data Not Found" + "$" + "400";
                                }
                                else
                                {
                                    return "Data Not Found" + "|" + "400";
                                }
                            }
                            else
                            {
                                if (urlMethod == "getVillageByOffice")
                                {
                                    return "Invalid Response" + "$VIPL" + "400";

                                }
                                else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                                {
                                    return "Invalid Response" + "$" + "400";

                                }
                                else
                                {
                                    return "Invalid Response" + "|" + "400";

                                }

                                //return "Invalid Response" + "|" + "400";
                            }
                        }
                    }

                    else if ((int)response.StatusCode == 500)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 - {responseBody}");
                        if (urlMethod == "getVillageByOffice")
                        {
                            return "Data Not Found" + "$VIPL" + "400";
                        }
                        else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
                        {
                            return "Data Not Found" + "$" + "400";
                        }
                        else
                        {
                            return "Data Not Found" + "|" + "400";
                        }
                    }
                    else
                    {
                        _logger.LogError($"Attempt {attempt}: Error - {response.StatusCode}");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return $"Error {response.StatusCode}: {responseContent}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Attempt {attempt}: Exception occurred - {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    _logger.LogWarning($"Retrying... Attempt {attempt + 1} in {delayMilliseconds}ms");
                    await Task.Delay(delayMilliseconds); // Wait before retrying
                }
            }
            _logger.LogError("All retry attempts failed.");

            if (urlMethod == "getVillageByOffice")
            {
                return "Data Not Found" + "$VIPL" + "400";
            }
            else if (urlMethod.ToLower() == "getregion" || urlMethod.ToLower() == "getdistrictbyregion")
            {
                return "Data Not Found" + "$" + "400";
            }
            else
            {
                return "Data Not Found" + "|" + "400";
            }

            //return "Data Not Found" + "|" + "400";
        }



        //public async Task<string> GetallDistrict(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("allDistrictList", HttpMethod.Post,_logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
        //        {
        //            var districts = System.Text.Json.JsonSerializer.Deserialize<List<EPICDistrict>>(response.Split("|")[0]);
        //            return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
        //        }
        //        else
        //        {
        //            return "Data List is Empty" +"|"+ response.Split("|")[1];
        //        }
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> GetallDistrict(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPICDistrict> dataList = new List<EPICDistrict>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT district_code,district_name FROM epcis.district WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPICDistrict()
                        {
                            district_code = reader["district_code"].ToString(),
                            district_name = reader["district_name"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("allDistrictList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                        {
                            var districts = System.Text.Json.JsonSerializer.Deserialize<List<EPICDistrict>>(response.Split("|")[0]);
                            return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                        }
                        else
                        {
                            return "Data List is Empty" + "|" + response.Split("|")[1];
                        }
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public async Task<string> getOfficeByDistrict(string district_code,ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("district_code", district_code);
            string response = await SendRequestAsync("getOfficeByDistrict", HttpMethod.Post,_logger,body);
            if (response.Split("|")[1] == "200")
            {
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var districts = JsonConvert.DeserializeObject<List<OfficeByDist>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }
                
            }
            else return response;
        }

        //public async Task<string> getVillageByOffice(string office_code,ILogger _logger)
        //{
        //    var body = new Dictionary<string, string>();
        //    body.Add("office_code", office_code);
        //    string response = await SendRequestAsync("getVillageByOffice", HttpMethod.Post, _logger,body);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var districts = JsonConvert.DeserializeObject<List<VillageByOffice>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //}

        public async Task<string> getVillageByOffice(string office_code, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("office_code", office_code);
            _logger.LogInformation("officecode: " + office_code);
            string response = await SendRequestAsync("getVillageByOffice", HttpMethod.Post, _logger, body);
            _logger.LogInformation("Fetch getVillageByOffice Data -> " + response);
            if (response.Split("$VIPL")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<VillageByOffice>>(response.Split("$VIPL")[0]);
                _logger.LogInformation("Districts: " + districts);
                return JsonConvert.SerializeObject(districts) + "$VIPL" + response.Split("$VIPL")[1];
            }
            else return response;
        }

        //public async Task<string> pdeApplicationTypeList(ILogger _logger)
        //{

        //    string response = await SendRequestAsync("pdeApplicationTypeList", HttpMethod.Post,_logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        _logger.LogInformation("In PDE Application type ");
        //        var districts = JsonConvert.DeserializeObject<List<EPCIApplicationType>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
        //    }
        //    else
        //        return response;
        //    //return response;

        //}

        public async Task<string> pdeApplicationTypeList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIApplicationType> dataList = new List<EPCIApplicationType>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT application_code,application_type FROM epcis.pde_application_type WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIApplicationType()
                        {
                            application_code = reader["application_code"].ToString(),
                            application_type = reader["application_type"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("pdeApplicationTypeList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        _logger.LogInformation("In PDE Application type ");
                        var districts = JsonConvert.DeserializeObject<List<EPCIApplicationType>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                    }
                    else
                        return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        public async Task<string> getMutationType(string mut_category, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("mut_category", mut_category);
            string response = await SendRequestAsync("getMutationType", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<EPCIMutationType>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;

        }

        public async Task<string> getDocListMutationtype(EPCISgetDocListMutationtype body, ILogger _logger)
        {
            //var body = new Dictionary<string, string>();
            //body.Add("mut_type", mut_type);
            string response = await SendRequestAsync("getDocListMutationtype", HttpMethod.Post, _logger,body);
            if (response.Split("|")[1] == "200")
            {
                var districts = System.Text.Json.JsonSerializer.Deserialize<List<EPCIDocument>>(response.Split("|")[0].ToString());//JsonConvert.DeserializeObject<List<EPCIDocListMutationtype>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;
        }

        //public async Task<string> applicationTypeList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("applicationTypeList", HttpMethod.Post,_logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var districts = JsonConvert.DeserializeObject<List<EPCIapplicationTypeList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> applicationTypeList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIapplicationTypeList> dataList = new List<EPCIapplicationTypeList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT application_type_code,application_type FROM epcis.application_type WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIapplicationTypeList()
                        {
                            applicant_category_code = reader["application_type_code"].ToString(),
                            applicant_category_type = reader["application_type"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("applicationTypeList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var districts = JsonConvert.DeserializeObject<List<EPCIapplicationTypeList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        public async Task<string> getCTSNoDetails(RequestCTSDetails body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getCTSNoDetails", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var districts = JsonConvert.DeserializeObject<List<EPCICTSNODetails>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }
            }
            else return response;
            //return response;

        }

        public async Task<string> getOwnerNameInfo(RequestOwnerNameInfo body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getOwnerNameInfo", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<EPCIOwnerNameInfo>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;

        }

        public async Task<string> getOwnerDetails(RequestOwnerDetails body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getOwnerDetails", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<EPCIOwnerDetails>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;

        }

        //public async Task<string> nameTitleList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("nameTitleList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var districts = JsonConvert.DeserializeObject<List<nameTitleList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}
        public async Task<string> nameTitleList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<nameTitleList> dataList = new List<nameTitleList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT name_title_code,name_title,name_title_english FROM epcis.name_title WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new nameTitleList()
                        {
                            name_title_code = reader["name_title_code"].ToString(),
                            name_title = reader["name_title"].ToString(),
                            name_title_english = reader["name_title_english"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("nameTitleList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var districts = JsonConvert.DeserializeObject<List<nameTitleList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public async Task<string> getFlatList(RequestCTSDetails body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getFlatList", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var districts = JsonConvert.DeserializeObject<List<EPCIFlatDetails>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }
            }
            else return response;
            //return response;

        }

        public async Task<string> getCTSDetails(RequestCTSDetails body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getCTSDetails", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var districts = JsonConvert.DeserializeObject<List<EPCICTSDetails>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }
            }
            else return response;
            //return response;

        }

        public async Task<string> getSroOfficeList(int district_code, ILogger _logger)
        {
            var body = new Dictionary<string, int>();
            body.Add("district_code", district_code);
            string response = await SendRequestAsync("getSroOfficeList", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<List<EPCISROOfficeList>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }


        public async Task<string> poaTypeList(ILogger _logger)
        {
            string response = await SendRequestAsync("poaTypeList", HttpMethod.Post, _logger);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<List<EPCIPOATypeList>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }

        public async Task<string> caseTypeList(string district_code, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("district_code", district_code);
            string response = await SendRequestAsync("caseTypeList", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<List<EPCICaseTypeList>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }

        //public async Task<string> deathCertificateList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("deathCertificateList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIDeathCertificateList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //}

        public async Task<string> deathCertificateList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIDeathCertificateList> dataList = new List<EPCIDeathCertificateList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT certificate_authority_code,certificate_authority_name FROM epcis.death_certificate WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIDeathCertificateList()
                        {
                            certificate_authority_code = reader["certificate_authority_code"].ToString(),
                            certificate_authority_name = reader["certificate_authority_name"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("deathCertificateList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIDeathCertificateList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        public async Task<string> ownerStatusOrCategory(ILogger _logger)
        {
            string response = await SendRequestAsync("ownerStatusOrCategory", HttpMethod.Post, _logger);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<List<EPCIOwnerStatusOrCategory>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }

        //public async Task<string> holderRelationList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("holderRelationList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIHolderRelationList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //}
        public async Task<string> holderRelationList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIHolderRelationList> dataList = new List<EPCIHolderRelationList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT relation_code,relation_name FROM epcis.holder_relation WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIHolderRelationList()
                        {
                            relation_code = reader["relation_code"].ToString(),
                            relation_name = reader["relation_name"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("holderRelationList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIHolderRelationList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //public async Task<string> genderList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("genderList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIGenderList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //}

        public async Task<string> genderList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIGenderList> dataList = new List<EPCIGenderList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT gender_code,gender_description FROM epcis.gender WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIGenderList()
                        {
                            gender_code = reader["gender_code"].ToString(),
                            gender_description = reader["gender_description"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("genderList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIGenderList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //public async Task<string> apkMasterList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("apkMasterList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIAPKMasterList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //}
        public async Task<string> apkMasterList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIAPKMasterList> dataList = new List<EPCIAPKMasterList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT apk_code,apk_description FROM epcis.apk_master WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIAPKMasterList()
                        {
                            apk_code = reader["apk_code"].ToString(),
                            apk_description = reader["apk_description"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("apkMasterList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIAPKMasterList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //public async Task<string> ownerAccountType(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("ownerAccountType", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIOwnerAccountTypeList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> ownerAccountType(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIOwnerAccountTypeList> dataList = new List<EPCIOwnerAccountTypeList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT account_type_code,account_type_description FROM epcis.account_type WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIOwnerAccountTypeList()
                        {
                            account_type_code = reader["account_type_code"].ToString(),
                            account_type_description = reader["account_type_description"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("ownerAccountType", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIOwnerAccountTypeList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //public async Task<string> bojaInstituteList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("bojaInstituteList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIBojaInstituteList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> bojaInstituteList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIBojaInstituteList> dataList = new List<EPCIBojaInstituteList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT institute_code,institute_description FROM epcis.boja_institute WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIBojaInstituteList()
                        {
                            institute_code = reader["institute_code"].ToString(),
                            institute_description = reader["institute_description"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("bojaInstituteList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIBojaInstituteList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        public async Task<string> getPropertyDetails(string village_code, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("village_code", village_code);
            string response = await SendRequestAsync("getPropertyDetails", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<List<EPCIPropertyDetails>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }


        public async Task<string> getULPINDetails(string ulpin, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("ulpin", ulpin);
            string response = await SendRequestAsync("getULPINDetails", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var getData = JsonConvert.DeserializeObject<EPCIULPINDetails>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
            }
            else return response;
        }


        //public async Task<string> getFloorTypeList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("getFloorTypeList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIFloorTypeList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> getFloorTypeList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIFloorTypeList> dataList = new List<EPCIFloorTypeList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT floor_type,floor_desc,floor_order_by FROM epcis.floor_type WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIFloorTypeList()
                        {
                            floor_type = Convert.ToInt32(reader["floor_type"].ToString()),
                            floor_desc = reader["floor_desc"].ToString(),
                            floor_order_by = reader["floor_order_by"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("getFloorTypeList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIFloorTypeList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //public async Task<string> getUnitTypeList(ILogger _logger)
        //{
        //    string response = await SendRequestAsync("getUnitTypeList", HttpMethod.Post, _logger);
        //    if (response.Split("|")[1] == "200")
        //    {
        //        var getData = JsonConvert.DeserializeObject<List<EPCIUnitTypeList>>(response.Split("|")[0]);
        //        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
        //    }
        //    else return response;
        //    //return response;

        //}

        public async Task<string> getUnitTypeList(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCIUnitTypeList> dataList = new List<EPCIUnitTypeList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT unit_code_156,unit_name_156 FROM epcis.unit_type WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCIUnitTypeList()
                        {
                            unit_code_156 = Convert.ToInt32(reader["unit_code_156"].ToString()),
                            unit_name_156 = reader["unit_name_156"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("getUnitTypeList", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        var getData = JsonConvert.DeserializeObject<List<EPCIUnitTypeList>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(getData) + "|" + response.Split("|")[1];
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        /*public async Task<string> GetallDistrict ()
        {
            // Set up the request URL
            string url = "https://api.mahabhumi.gov.in/api/epcis/allDistrictList"; // Replace with your API endpoint

            // Create a new HttpRequestMessage
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Set up headers
            //Production
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "0uXCpuv3E1ZDnJRDO7xJiQhHuaM8PjYC71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa");
            request.Headers.Add("API-KEY", "e06ae416-d3db-4b7e-8cbd-edc73f6c706a");
            request.Headers.Add("SECRET-KEY", "ObvLNRgvtbIYe2IC9t3IpzEmclzKEjkqHdjM6iP7ggN76zSoWP9M1pekkdYDEb7c");

            //Local
            *//*request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "URqyR0fRlC3B9dxaAlTR1Ra31QKZ9HHnVaTPMihixMlbvKnhCJPAtQ3qYPnCKbIB");
            request.Headers.Add("API-KEY", "f3c040ae-4264-f1d1-ac58-486e2453");
            request.Headers.Add("SECRET-KEY", "9z3g7YaHCzwj4diHacM2Cdt8Cg1FOYVLjh2nOtRjGBz67Ygh3UiYzwcOe5By");
*//*
            // Send the request
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseBody);
                var jsonobject = JsonConvert.DeserializeObject<LgdApiResponse>(responseBody);
                var decrypted = decryptor.DecryptData(jsonobject!.Data);
                var districts = JsonConvert.DeserializeObject<List<LGDDistrict>>(decrypted);
                return JsonConvert.SerializeObject(districts) +"~"+(int)response.StatusCode;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent+"~"+(int)response.StatusCode;
            }
        }
*/



        //Gauri W
        //pincode

        //region
        public async Task<string> GetRegion(ILogger _logger)
        {
            string response = await SendRequestAsync("getregion", HttpMethod.Post, _logger);
            if (response.Split("$")[1] == "200")
            {
                if (response.Split("$")[0] != null && response.Split("$")[0].ToList().Count > 0)
                {

                    var regions = JsonConvert.DeserializeObject<List<EPCIRegion>>(response.Split("$")[0]);
                    _logger.LogInformation("Regions Data: " + regions);
                    return JsonConvert.SerializeObject(regions) + "$" + response.Split("$")[1];
                }
                else
                {
                    return "Region List is Empty" + "$" + response.Split("$")[1];
                }
            }
            else return response;
        }

        public async Task<string> GetDistrictByRegion(int regionCode, ILogger _logger)
        {
            var body = new Dictionary<string, int>();
            body.Add("region_code", regionCode);
            _logger.LogInformation("region_code: " + regionCode);
            string response = await SendRequestAsync("getDistrictByRegion", HttpMethod.Post, _logger, body);
            _logger.LogInformation("Fetch getDistrictByRegion Data -> " + response);
            if (response.Split("$")[1] == "200")
            {
                var DistrictsByRegion = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(response.Split("$")[0]);
                _logger.LogInformation("Districts By Region: " + DistrictsByRegion);
                return JsonConvert.SerializeObject(DistrictsByRegion) + "$" + response.Split("$")[1];
            }
            else return response;
        }


        //get application id count

        public async Task<Dictionary<string, int>?> FetchCountOfApplicationsAsync(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

                string regionCode = getAllApplicationIdForReport.region_code!;
                string districtCode = getAllApplicationIdForReport.district_code!;
                string officeCode = getAllApplicationIdForReport.office_code!;

                Dictionary<string, int> result = new Dictionary<string, int>();

                var expectedStatusCodes = new Dictionary<int, string>
                {
                    { 0, "Partially Submitted/Pending" },
                    { 10, "Application is submitted to EPCIS" },
                    { 11, "Truti Patra is generated" },
                    { 12, "Application is rejected" },
                    { 13, "Nikali Patra is generated" },
                    { 14, "Notice 9 is generated" },
                    { 15, "Inward Number Error" }
                };

                IQueryable<ApplicationDTL> query = context.applicationDTL
                    .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate);


                // All regions
                if (regionCode == "0" && districtCode == "0" && officeCode == "0")
                {
                    var regionActualCounts = query
                        .GroupBy(a => a.status >= 1 && a.status <= 9 ? 0 : a.status)
                        .Select(g => new
                        {
                            StatusCode = g.Key,
                            Count = g.Count()
                        })
                        .ToList();

                    result = expectedStatusCodes
                        .Select(kvp =>
                        {
                            var count = regionActualCounts.FirstOrDefault(a => a.StatusCode == kvp.Key)?.Count ?? 0;
                            var key = kvp.Value.Replace(" ", "_").Replace("/", "_").Replace("-", "_").Replace(",", "");
                            return new KeyValuePair<string, int>(key, count);
                        })
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


                    //var applicationIds = query.Where(x=>x.status==10).Select(x => x.applicationid).ToList();

                    //int inwardNoGeneratedApplications = context.nICAPIResponses.Where(c => applicationIds.Contains(c.applicationid) &&
                    //c.createddatetime>=fromDate && c.createddatetime<=toDate && System.Text.RegularExpressions.Regex.IsMatch(c.inwardno!, @"^[0-9]+$")).Count();

                    result.Add("total", result.Values.Sum());
                    //result.Add("Application_is_submitted_to_EPCIS", inwardNoGeneratedApplications);
                    return result;
                }
                // Specific region (with all districts & offices)
                else if (regionCode != "0" && districtCode == "0" && officeCode == "0")
                {
                    var districtsData = await GetDistrictByRegion(Convert.ToInt32(regionCode), _logger);
                    var parts = districtsData.Split('$');
                    if (parts.Length != 2 || !int.TryParse(parts[1], out int statusCode) || statusCode != 200)
                        return null;

                    var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);
                    var districtCodes = districtList!
                    .Select(d =>
                    {
                        int code = d.district_code;
                        return code <= 9 ? "0" + code.ToString() : code.ToString();
                    })
                    .ToList();
                    query = query.Where(s => districtCodes.Contains(s.district_code!));

                    //foreach (var districtcode in districtCodes)
                    //{
                    //    var talukaData = await getOfficeByDistrict(districtcode, _logger);
                    //    parts = talukaData.Split('$');
                    //    if (parts.Length != 2 || !int.TryParse(parts[1], out statusCode) || statusCode != 200)
                    //        return null;

                    //    var talukaList = JsonConvert.DeserializeObject<List<OfficeByDist>>(parts[0]);
                    //    var talukaCodes = talukaList!.Select(d => d.office_code!.ToString()).ToList();

                    //    query = query.Where(s => districtCodes.Contains(s.district_code!) && talukaCodes.Contains(s.office_code!));
                    //}
                }
                else if (regionCode != "0" && districtCode != "0" && officeCode == "0")
                {
                    if (Convert.ToInt32(districtCode) <= 9)
                    {
                        districtCode = "0" + districtCode;
                    }
                    query = query.Where(s => s.district_code == districtCode);

                }
                else if (regionCode != "0" && districtCode != "0" && officeCode != "0")
                {

                    if (Convert.ToInt32(districtCode) <= 9)
                    {
                        districtCode = "0" + districtCode;
                    }
                    query = query.Where(s => s.district_code == districtCode && s.office_code == officeCode);

                }
                else
                {
                    return null;
                }

                var actualCounts = query
                    .GroupBy(a => a.status >= 1 && a.status <= 9 ? 0 : a.status)
                    .Select(g => new
                    {
                        StatusCode = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                result = expectedStatusCodes
                   .Select(kvp =>
                   {
                       var count = actualCounts.FirstOrDefault(a => a.StatusCode == kvp.Key)?.Count ?? 0;
                       var key = kvp.Value.Replace(" ", "_").Replace("/", "_").Replace("-", "_").Replace(",", "");
                       return new KeyValuePair<string, int>(key, count);
                   })
                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                result.Add("total", result.Values.Sum());

                return result;
            }
            catch (Exception ex)
            {
                throw new HandleException($"Fetch failed: {ex.Message}");
            }
        }


        //fetch mutation count by region, district and taluka
        //public async Task<Dictionary<string, int>?> FetchMutationCountAsync(GetAllApplicationIdForReport getAllApplicationIdForReport)
        //{
        //    try
        //    {
        //        DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
        //        DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

        //        string regionCode = getAllApplicationIdForReport.region_code!;
        //        string districtCode = getAllApplicationIdForReport.district_code!;
        //        string officeCode = getAllApplicationIdForReport.office_code!;

        //        Dictionary<string, int> result = new Dictionary<string, int>();
        //        List<int> status = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        //        IQueryable<ApplicationDTL> query = context.applicationDTL
        //            .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate && status.Contains(s.status));

        //        // All regions
        //        if (regionCode == "0" && districtCode == "0" && officeCode == "0")
        //        {
        //            var rawDataa = await query.Select(s => new
        //            {
        //                MutationName = s.mutation_type_name ?? "Unknown",
        //                StatusCode = s.status >= 1 && s.status <= 9 ? 0 : s.status
        //            }).ToListAsync();

        //            result = rawDataa
        //               .GroupBy(x => new { x.MutationName, x.StatusCode })
        //               .Select(g => new
        //               {
        //                   MutationName = GetShortMutationName(g.Key.MutationName),
        //                   StatusCode = g.Key.StatusCode,
        //                   Count = g.Count()
        //               })
        //               .GroupBy(x => x.MutationName)
        //               .ToDictionary(
        //                   g => g.Key,
        //                   g => g.Sum(x => x.Count)
        //               );
        //        }
        //        // Specific region (all districts and offices)
        //        else if (regionCode != "0" && districtCode == "0" && officeCode == "0")
        //        {
        //            var districtsData = await GetDistrictByRegion(Convert.ToInt32(regionCode), _logger);
        //            var parts = districtsData.Split('$');

        //            if (parts.Length != 2 || !int.TryParse(parts[1], out int statusCode) || statusCode != 200)
        //                return null;

        //            var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);
        //            var districtCodes = districtList!
        //                .Select(d => d.district_code <= 9 ? $"0{d.district_code}" : d.district_code.ToString())
        //                .ToList();

        //            query = query.Where(s => districtCodes.Contains(s.district_code!));
        //        }
        //        // Region + district only
        //        else if (regionCode != "0" && districtCode != "0" && officeCode == "0")
        //        {
        //            if (Convert.ToInt32(districtCode) <= 9)
        //                districtCode = "0" + districtCode;

        //            query = query.Where(s => s.district_code == districtCode);
        //        }
        //        // Region + district + office
        //        else if (regionCode != "0" && districtCode != "0" && officeCode != "0")
        //        {
        //            if (Convert.ToInt32(districtCode) <= 9)
        //                districtCode = "0" + districtCode;

        //            query = query.Where(s => s.district_code == districtCode && s.office_code == officeCode);
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //        // Common data extraction and grouping
        //        var rawData = await query.Select(s => new
        //        {
        //            MutationName = s.mutation_type_name ?? "Unknown",
        //            StatusCode = s.status >= 1 && s.status <= 9 ? 0 : s.status
        //        }).ToListAsync();

        //         result = rawData
        //            .GroupBy(x => new { x.MutationName, x.StatusCode })
        //            .Select(g => new
        //            {
        //                MutationName = GetShortMutationName(g.Key.MutationName),
        //                StatusCode = g.Key.StatusCode,
        //                Count = g.Count()
        //            })
        //            .GroupBy(x => x.MutationName)
        //            .ToDictionary(
        //                g => g.Key,
        //                g => g.Sum(x => x.Count)
        //            );

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException($"Fetch failed: {ex.Message}");
        //    }
        //}

        public async Task<List<FetchCountOfMutations>> FetchMutationCountAsync(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

                string regionCode = getAllApplicationIdForReport.region_code!;
                string districtCode = getAllApplicationIdForReport.district_code!;
                string officeCode = getAllApplicationIdForReport.office_code!;

                List<int> partiallySubmitStatusCodes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                List<int> includedStatusCodes = new List<int> { 11, 13, 14 };

                IQueryable<ApplicationDTL> query = context.applicationDTL
                    .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate && !partiallySubmitStatusCodes.Contains(s.status)
                    && s.status > 9);


                // Apply region/district/office filtering
                if (regionCode != "0" && districtCode == "0" && officeCode == "0")
                {
                    var districtsData = await GetDistrictByRegion(Convert.ToInt32(regionCode), _logger);
                    var parts = districtsData.Split('$');

                    var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);
                    var districtCodes = districtList!
                        .Select(d => d.district_code <= 9 ? $"0{d.district_code}" : d.district_code.ToString())
                        .ToList();

                    query = query.Where(s => districtCodes.Contains(s.district_code!));
                }
                else if (regionCode != "0" && districtCode != "0" && officeCode == "0")
                {
                    if (Convert.ToInt32(districtCode) <= 9)
                        districtCode = "0" + districtCode;

                    query = query.Where(s => s.district_code == districtCode);
                }
                else if (regionCode != "0" && districtCode != "0" && officeCode != "0")
                {
                    if (Convert.ToInt32(districtCode) <= 9)
                        districtCode = "0" + districtCode;

                    query = query.Where(s => s.district_code == districtCode && s.office_code == officeCode);
                }

                var rawData = query
                //.Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate)
                .Select(s => new
                {
                    MutationName = s.mutation_type_name ?? "Unknown",
                    //StatusCode = s.status >= 11 && s.status <= 15 ? 0 : s.status
                    StatusCode = includedStatusCodes.Contains(s.status) ? 0 : s.status
                    //StatusCode = s.status
                })
                .ToList(); // Execute query here

                var result = rawData
                    .GroupBy(x => new { x.MutationName, x.StatusCode })
                    .Select(g => new
                    {
                        MutationName = GetShortMutationName(g.Key.MutationName.Trim().ToLower()), //GetShortMutationName(g.Key.MutationName),
                        StatusCode = g.Key.StatusCode,
                        Count = g.Count()
                    })
                    .GroupBy(x => x.MutationName)
                    .Select(g => new FetchCountOfMutations
                    {
                        MutationName = g.Key,
                        Statuses = g.Select(x => new StatusDetail
                        {
                            ApplicationStatusCode = x.StatusCode,
                            ApplicationStatus = x.StatusCode == 0 ? "Application Processed by EPCIS" :
                                                x.StatusCode == 10 ? "Application is submitted to EPCIS" :
                                                x.StatusCode == 12 ? "Application is Rejected" :
                                                x.StatusCode == 15 ? "Inward Number Error" :
                                                "Unknown",
                            CountOfMutation = x.Count
                        }).ToList(),
                        CountOfMutation = 0 // Optional: or g.Sum(x => x.Count) if needed
                    })
                    .ToList();
                if (result == null)
                {
                    return result = null;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new HandleException($"Fetch failed: {ex.Message}");
            }
        }

        private string GetShortMutationName(string fullName)
        {
            return fullName switch
            {

                "गहाणखत / तारण / बोजा दाखल नोंद" => "गहाणखत नोंद",
                _ => fullName // default to original if no match
            };
        }



        public byte[] ExportToExcel(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {

            DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
            DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

            string regionCode = getAllApplicationIdForReport.region_code!;
            string districtCode = getAllApplicationIdForReport.district_code!;
            string officeCode = getAllApplicationIdForReport.office_code!;


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Applications");


            var rawData = context.mutationCTSNoDTLs
                .Include(m => m.applicationDTL)
                .Where(m => m.applicationDTL != null &&
                            m.applicationDTL.createddatetime >= fromDate &&
                            m.applicationDTL.createddatetime <= toDate)
                .AsEnumerable() // Switch to in-memory for grouping
                .GroupBy(m => m.applicationDTL!.applicationid)
                .Select(g => g.OrderByDescending(m => m.applicationDTL!.createddatetime).First())
                .ToList(); // Materialize here

            if (regionCode != "0" && districtCode == "0" && officeCode == "0")
            {
                var districtsData = GetDistrictByRegion(Convert.ToInt32(regionCode), _logger).Result;
                var parts = districtsData.Split('$');

                if (parts.Length == 2 && int.TryParse(parts[1], out int statusCode) && statusCode == 200)
                {
                    var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);
                    var districtCodes = districtList!
                        .Select(d => d.district_code <= 9 ? $"0{d.district_code}" : d.district_code.ToString())
                        .ToList();

                    rawData = rawData
                        .Where(s => districtCodes.Contains(s.applicationDTL!.district_code!))
                        .ToList();
                }
            }
            else if (regionCode != "0" && districtCode != "0" && officeCode == "0")
            {
                if (Convert.ToInt32(districtCode) <= 9)
                {
                    districtCode = "0" + districtCode;
                }
                rawData = rawData
                    .Where(s => s.applicationDTL!.district_code == districtCode)
                    .ToList();
            }
            else if (regionCode != "0" && districtCode != "0" && officeCode != "0")
            {
                if (Convert.ToInt32(districtCode) <= 9)
                {
                    districtCode = "0" + districtCode;
                }
                rawData = rawData
                    .Where(s => s.applicationDTL!.district_code == districtCode &&
                                s.applicationDTL!.office_code == officeCode)
                    .ToList();
            }


            // Headers

            worksheet.Cell(1, 1).Value = "Application ID";
            worksheet.Cell(1, 2).Value = "Inward No";
            worksheet.Cell(1, 3).Value = "Status";
            //worksheet.Cell(1, 4).Value = "Mutation Type Code";
            worksheet.Cell(1, 4).Value = "Mutation Name";
            //worksheet.Cell(1, 6).Value = "District Code";
            worksheet.Cell(1, 5).Value = "District Name In Marathi";
            //worksheet.Cell(1, 8).Value = "District Name In English";
            //worksheet.Cell(1, 9).Value = "Office / Taluka Code";
            worksheet.Cell(1, 6).Value = "Office / Taluka Name";
            worksheet.Cell(1, 7).Value = "Village Name in Marathi";
            worksheet.Cell(1, 8).Value = "Application Created Date";
            worksheet.Cell(1, 9).Value = "Is Deleted";
            int row = 2;

            rawData = rawData.OrderByDescending(m => m.applicationDTL!.createddatetime).ToList();

            foreach (var item in rawData)
            {
                worksheet.Cell(row, 1).Value = item.applicationDTL!.applicationid!.ToString();
                worksheet.Cell(row, 1).Style.NumberFormat.Format = "@"; // text format

                var inwardNo = item.applicationDTL!.inwardno;
                string formattedInwardNo = inwardNo != "NA" ? $"{inwardNo!.Substring(0, 4)}/{inwardNo.Substring(4, 4)}/{inwardNo.Substring(8)}" : "NA";

                var isDeleted = item.applicationDTL!.isDeleted;
                string formattedisDeleted = isDeleted == true ? "Deleted" : "Not Deleted";

                worksheet.Cell(row, 2).Value = formattedInwardNo;
                worksheet.Cell(row, 3).Value = GetStatusText(item.applicationDTL!.status, item.applicationDTL!.inwardno!);
                //worksheet.Cell(row, 4).Value = item.mutation_type_code;
                worksheet.Cell(row, 4).Value = item.applicationDTL!.mutation_type_name;
                //worksheet.Cell(row, 6).Value = item.district_code;
                worksheet.Cell(row, 5).Value = item.applicationDTL!.district_name_in_marathi;
                //worksheet.Cell(row, 8).Value = item.district_name_in_english;
                //worksheet.Cell(row, 9).Value = item.office_code;
                worksheet.Cell(row, 6).Value = item.applicationDTL!.office_name;
                worksheet.Cell(row, 7).Value = item.village_or_peth_name;

                worksheet.Cell(row, 8).Value = item.applicationDTL!.createddatetime.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 9).Value = formattedisDeleted;

                row++;
            }

            var headerRange = worksheet.Range("A1:I1");
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Apricot;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportToExcelPerticularApplicationStatusWise(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {

            DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
            DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

            string regionCode = getAllApplicationIdForReport.region_code!;
            string districtCode = getAllApplicationIdForReport.district_code!;
            string officeCode = getAllApplicationIdForReport.office_code!;
            int? statusId = getAllApplicationIdForReport.statusId;
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("StatusWiseApplicationIds");

            var rawData = context.mutationCTSNoDTLs
                .Include(m => m.applicationDTL)
                .Where(m => m.applicationDTL != null &&
                            m.applicationDTL.createddatetime >= fromDate &&
                            m.applicationDTL.createddatetime <= toDate)
                .AsEnumerable()
                .GroupBy(m => m.applicationDTL!.applicationid)
                .Select(g => g.OrderByDescending(m => m.applicationDTL!.createddatetime).First())
                .ToList();


            if (statusId == 10)
            {

                //var applicationIds = context.nICAPIResponses.Where(c => c.createddatetime >= fromDate && c.createddatetime <= toDate
                //              && System.Text.RegularExpressions.Regex.IsMatch(c.inwardno!, @"^[0-9]+$")).Select(c => c.applicationid).ToList();

                //var status = new List<int> { 11, 12, 13, 14, 15 };
                rawData = rawData.Where(x => x.applicationDTL.status == 10).ToList();
            }
            else if (statusId == 11)
            {
                rawData = rawData.Where(x => x.applicationDTL.status == 11).ToList();

            }
            else if (statusId == 12)
            {
                rawData = rawData.Where(x => x.applicationDTL!.status == 12).ToList();

            }
            else if (statusId == 13)
            {
                rawData = rawData.Where(x => x.applicationDTL!.status == 13).ToList();

            }
            else if (statusId == 14)
            {
                rawData = rawData.Where(x => x.applicationDTL!.status == 14).ToList();

            }
            else if (statusId == 15)
            {
                rawData = rawData.Where(x => x.applicationDTL!.status == 15).ToList();

            }

            if (regionCode != "0" && districtCode == "0" && officeCode == "0")
            {
                var districtsData = GetDistrictByRegion(Convert.ToInt32(regionCode), _logger).Result;
                var parts = districtsData.Split('$');

                if (parts.Length == 2 && int.TryParse(parts[1], out int statusCode) && statusCode == 200)
                {
                    var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);
                    var districtCodes = districtList!
                        .Select(d => d.district_code <= 9 ? $"0{d.district_code}" : d.district_code.ToString())
                        .ToList();

                    rawData = rawData
                        .Where(s => districtCodes.Contains(s.applicationDTL!.district_code!))
                        .ToList();
                }
            }
            else if (regionCode != "0" && districtCode != "0" && officeCode == "0")
            {
                if (Convert.ToInt32(districtCode) <= 9)
                {
                    districtCode = "0" + districtCode;
                }
                rawData = rawData
                    .Where(s => s.applicationDTL!.district_code == districtCode)
                    .ToList();
            }
            else if (regionCode != "0" && districtCode != "0" && officeCode != "0")
            {
                if (Convert.ToInt32(districtCode) <= 9)
                {
                    districtCode = "0" + districtCode;
                }
                rawData = rawData
                    .Where(s => s.applicationDTL!.district_code == districtCode &&
                                s.applicationDTL!.office_code == officeCode)
                    .ToList();
            }

            // Headers

            worksheet.Cell(1, 1).Value = "Application ID";
            worksheet.Cell(1, 2).Value = "Inward No";
            worksheet.Cell(1, 3).Value = "Status";
            //worksheet.Cell(1, 4).Value = "Mutation Type Code";
            worksheet.Cell(1, 4).Value = "Mutation Name";
            //worksheet.Cell(1, 6).Value = "District Code";
            worksheet.Cell(1, 5).Value = "District Name In Marathi";
            //worksheet.Cell(1, 8).Value = "District Name In English";
            //worksheet.Cell(1, 9).Value = "Office / Taluka Code";
            worksheet.Cell(1, 6).Value = "Office / Taluka Name";
            worksheet.Cell(1, 7).Value = "Village Name in Marathi";
            worksheet.Cell(1, 8).Value = "Application Created Date";
            worksheet.Cell(1, 9).Value = "Is Deleted";
            int row = 2;

            rawData = rawData.OrderByDescending(m => m.applicationDTL!.createddatetime).ToList();

            foreach (var item in rawData)
            {
                worksheet.Cell(row, 1).Value = item.applicationDTL!.applicationid!.ToString();
                worksheet.Cell(row, 1).Style.NumberFormat.Format = "@"; // text format

                var inwardNo = item.applicationDTL!.inwardno;
                string formattedInwardNo = inwardNo != "NA" ? $"{inwardNo!.Substring(0, 4)}/{inwardNo.Substring(4, 4)}/{inwardNo.Substring(8)}" : "NA";

                var isDeleted = item.applicationDTL!.isDeleted;
                string formattedisDeleted = isDeleted == true ? "Deleted" : "Not Deleted";

                worksheet.Cell(row, 2).Value = formattedInwardNo;
                worksheet.Cell(row, 3).Value = GetStatusText(item.applicationDTL!.status, item.applicationDTL!.inwardno!);
                //worksheet.Cell(row, 4).Value = item.mutation_type_code;
                worksheet.Cell(row, 4).Value = item.applicationDTL!.mutation_type_name;
                //worksheet.Cell(row, 6).Value = item.district_code;
                worksheet.Cell(row, 5).Value = item.applicationDTL!.district_name_in_marathi;
                //worksheet.Cell(row, 8).Value = item.district_name_in_english;
                //worksheet.Cell(row, 9).Value = item.office_code;
                worksheet.Cell(row, 6).Value = item.applicationDTL!.office_name;
                worksheet.Cell(row, 7).Value = item.village_or_peth_name;

                worksheet.Cell(row, 8).Value = item.applicationDTL!.createddatetime.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 9).Value = formattedisDeleted;

                row++;
            }

            var headerRange = worksheet.Range("A1:I1");
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Apricot;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        private string GetStatusText(int status, string inwardNo)
        {
            if (status >= 1 && status <= 9 && inwardNo == "NA")
            {
                return "Partially Submitted / Pending";
            }

            return (status, inwardNo) switch
            {
                (10, not "NA") => "Application is submitted to EPCIS",
                (11, not "NA") => "Truti Patra is generated",
                (12, not "NA") => "Application is rejected",
                (13, not "NA") => "Nikali Patra is generated",
                (14, not "NA") => "Notice 9 is generated",
                (15, "NA") => "Inward No Error",
                _ => "Unknown"
            };
        }
        //public List<PostOffice>? GetAndSavePincodeData(string pincode)
        //{
        //    List<pinCodeMaster> pincodesList = new List<pinCodeMaster>();
        //    List<PostOffice>? fetchDataList = null;

        //    pincodesList = context.pincodemaster.Where(s => s.Pincode == pincode).ToList();
        //    fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //    {
        //        Name = pincodesList.Name,
        //        Description = pincodesList.Description,
        //        BranchType = pincodesList.BranchType,
        //        DeliveryStatus = pincodesList.DeliveryStatus,
        //        Circle = pincodesList.Circle,
        //        District = pincodesList.District,
        //        Division = pincodesList.Division,
        //        Region = pincodesList.Region,
        //        Block = pincodesList.Block,
        //        State = pincodesList.State,
        //        Country = pincodesList.Country,
        //        Pincode = pincodesList.Pincode
        //    }).ToList();

        //    return fetchDataList;
        //}


        //public async Task<List<PostOffice>?> GetAndSavePincodeData(string pincode)
        //{
        //    string url = $"https://api.postalpincode.in/pincode/{pincode}";
        //    //string url = "https://api.postalpincode.in/pincode/411033";
        //    HttpResponseMessage response = await client.GetAsync(url);

        //    List<pinCodeApiResponseTbl> pincodesList = new List<pinCodeApiResponseTbl>();
        //    List<PostOffice>? fetchDataList = null;


        //    if (!response.IsSuccessStatusCode)
        //    {
        //        pincodesList = context.pinCodeApiResponseTbl.Where(s=>s.Pincode == pincode).ToList();

        //        fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //        {
        //            Name = pincodesList.Name,
        //            Description = pincodesList.Description,
        //            BranchType = pincodesList.BranchType,
        //            DeliveryStatus = pincodesList.DeliveryStatus,
        //            Circle = pincodesList.Circle,
        //            District = pincodesList.District,
        //            Division = pincodesList.Division,
        //            Region = pincodesList.Region,
        //            Block = pincodesList.Block,
        //            State = pincodesList.State,
        //            Country = pincodesList.Country,
        //            Pincode = pincodesList.Pincode
        //        }).ToList();

        //        return fetchDataList;

        //        //throw new Exception($"Error fetching data: {response.StatusCode}");
        //    }
        //    else
        //    {
        //        string jsonString = await response.Content.ReadAsStringAsync();
        //        var resultList = JsonSerializer.Deserialize<List<pincodeApiRes>>(jsonString, new JsonSerializerOptions // Deserialize JSON as a List<pincodeApiRes>
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        if(resultList != null && resultList.Count>0)
        //        {
        //            //show data from their api direct
        //            foreach(var pincodedata in resultList)
        //            {
        //                foreach(var postoffc in pincodedata.PostOffice!)
        //                {

        //                    var existingRecord = await context.pinCodeApiResponseTbl.FirstOrDefaultAsync(s => s.Pincode == postoffc.Pincode && s.Name == postoffc.Name);
        //                    if (existingRecord == null)
        //                    {
        //                        var newPincodeData = new pinCodeApiResponseTbl
        //                        {
        //                            Name = postoffc.Name,
        //                            Description = postoffc.Description,
        //                            BranchType = postoffc.BranchType,
        //                            DeliveryStatus = postoffc.DeliveryStatus,
        //                            Circle = postoffc.Circle,
        //                            District = postoffc.District,
        //                            Division = postoffc.Division,
        //                            Region = postoffc.Region,
        //                            Block = postoffc.Block,
        //                            State = postoffc.State,
        //                            Country = postoffc.Country,
        //                            Pincode = postoffc.Pincode
        //                        };

        //                        context.pinCodeApiResponseTbl.Add(newPincodeData);
        //                    }
        //                }
        //            }

        //            await context.SaveChangesAsync();

        //            pincodesList = context.pinCodeApiResponseTbl.Where(s => s.Pincode == pincode).ToList();
        //            fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //            {
        //                Name = pincodesList.Name,
        //                Description = pincodesList.Description,
        //                BranchType = pincodesList.BranchType,
        //                DeliveryStatus = pincodesList.DeliveryStatus,
        //                Circle = pincodesList.Circle,
        //                District = pincodesList.District,
        //                Division = pincodesList.Division,
        //                Region = pincodesList.Region,
        //                Block = pincodesList.Block,
        //                State = pincodesList.State,
        //                Country = pincodesList.Country,
        //                Pincode = pincodesList.Pincode
        //            }).ToList();

        //            return fetchDataList;
        //            //return fetchDataList;

        //        }
        //        else
        //        {
        //            pincodesList = context.pinCodeApiResponseTbl.Where(s => s.Pincode == pincode).ToList();

        //            fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //            {
        //                Name = pincodesList.Name,
        //                Description = pincodesList.Description,
        //                BranchType = pincodesList.BranchType,
        //                DeliveryStatus = pincodesList.DeliveryStatus,
        //                Circle = pincodesList.Circle,
        //                District = pincodesList.District,
        //                Division = pincodesList.Division,
        //                Region = pincodesList.Region,
        //                Block = pincodesList.Block,
        //                State = pincodesList.State,
        //                Country = pincodesList.Country,
        //                Pincode = pincodesList.Pincode
        //            }).ToList();

        //            return fetchDataList;
        //        }
        //    }
        //}



        //new 
        //GW
        //return only count 
        //Dictionary<string, int>
        //List<FetchCountOfApplication>

        public List<PostOffice>? GetAndSavePincodeData(string pincode)
        {
            List<pinCodeMaster> pincodesList = new List<pinCodeMaster>();
            List<PostOffice>? fetchDataList = null;

            pincodesList = context.pincodemaster.Where(s => s.Pincode == pincode).ToList();
            fetchDataList = pincodesList.Select(pincodesList => new PostOffice
            {
                Name = pincodesList.Name,
                Description = pincodesList.Description,
                BranchType = pincodesList.BranchType,
                DeliveryStatus = pincodesList.DeliveryStatus,
                Circle = pincodesList.Circle,
                District = pincodesList.District,
                Division = pincodesList.Division,
                Region = pincodesList.Region,
                Block = pincodesList.Block,
                State = pincodesList.State,
                Country = pincodesList.Country,
                Pincode = pincodesList.Pincode
            }).ToList();
            
            return fetchDataList;
        }

        //public async Task<List<PostOffice>?> GetAndSavePincodeData(string pincode)
        //{
        //    string url = $"https://api.postalpincode.in/pincode/{pincode}";
        //    //string url = "https://api.postalpincode.in/pincode/411033";
        //    HttpResponseMessage response = await client.GetAsync(url);

        //    List<pinCodeApiResponseTbl> pincodesList = new List<pinCodeApiResponseTbl>();
        //    List<PostOffice>? fetchDataList = null;


        //    if (!response.IsSuccessStatusCode)
        //    {
        //        pincodesList = context.pinCodeApiResponseTbl.Where(s=>s.Pincode == pincode).ToList();

        //        fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //        {
        //            Name = pincodesList.Name,
        //            Description = pincodesList.Description,
        //            BranchType = pincodesList.BranchType,
        //            DeliveryStatus = pincodesList.DeliveryStatus,
        //            Circle = pincodesList.Circle,
        //            District = pincodesList.District,
        //            Division = pincodesList.Division,
        //            Region = pincodesList.Region,
        //            Block = pincodesList.Block,
        //            State = pincodesList.State,
        //            Country = pincodesList.Country,
        //            Pincode = pincodesList.Pincode
        //        }).ToList();

        //        return fetchDataList;

        //        //throw new Exception($"Error fetching data: {response.StatusCode}");
        //    }
        //    else
        //    {
        //        string jsonString = await response.Content.ReadAsStringAsync();
        //        var resultList = JsonSerializer.Deserialize<List<pincodeApiRes>>(jsonString, new JsonSerializerOptions // Deserialize JSON as a List<pincodeApiRes>
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        if(resultList != null && resultList.Count>0)
        //        {
        //            //show data from their api direct
        //            foreach(var pincodedata in resultList)
        //            {
        //                foreach(var postoffc in pincodedata.PostOffice!)
        //                {

        //                    var existingRecord = await context.pinCodeApiResponseTbl.FirstOrDefaultAsync(s => s.Pincode == postoffc.Pincode && s.Name == postoffc.Name);
        //                    if (existingRecord == null)
        //                    {
        //                        var newPincodeData = new pinCodeApiResponseTbl
        //                        {
        //                            Name = postoffc.Name,
        //                            Description = postoffc.Description,
        //                            BranchType = postoffc.BranchType,
        //                            DeliveryStatus = postoffc.DeliveryStatus,
        //                            Circle = postoffc.Circle,
        //                            District = postoffc.District,
        //                            Division = postoffc.Division,
        //                            Region = postoffc.Region,
        //                            Block = postoffc.Block,
        //                            State = postoffc.State,
        //                            Country = postoffc.Country,
        //                            Pincode = postoffc.Pincode
        //                        };

        //                        context.pinCodeApiResponseTbl.Add(newPincodeData);
        //                    }
        //                }
        //            }

        //            await context.SaveChangesAsync();

        //            pincodesList = context.pinCodeApiResponseTbl.Where(s => s.Pincode == pincode).ToList();
        //            fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //            {
        //                Name = pincodesList.Name,
        //                Description = pincodesList.Description,
        //                BranchType = pincodesList.BranchType,
        //                DeliveryStatus = pincodesList.DeliveryStatus,
        //                Circle = pincodesList.Circle,
        //                District = pincodesList.District,
        //                Division = pincodesList.Division,
        //                Region = pincodesList.Region,
        //                Block = pincodesList.Block,
        //                State = pincodesList.State,
        //                Country = pincodesList.Country,
        //                Pincode = pincodesList.Pincode
        //            }).ToList();

        //            return fetchDataList;
        //            //return fetchDataList;

        //        }
        //        else
        //        {
        //            pincodesList = context.pinCodeApiResponseTbl.Where(s => s.Pincode == pincode).ToList();

        //            fetchDataList = pincodesList.Select(pincodesList => new PostOffice
        //            {
        //                Name = pincodesList.Name,
        //                Description = pincodesList.Description,
        //                BranchType = pincodesList.BranchType,
        //                DeliveryStatus = pincodesList.DeliveryStatus,
        //                Circle = pincodesList.Circle,
        //                District = pincodesList.District,
        //                Division = pincodesList.Division,
        //                Region = pincodesList.Region,
        //                Block = pincodesList.Block,
        //                State = pincodesList.State,
        //                Country = pincodesList.Country,
        //                Pincode = pincodesList.Pincode
        //            }).ToList();

        //            return fetchDataList;
        //        }
        //    }
        //}



        //new 
        //GW
        //return only count 
        //Dictionary<string, int>
        //List<FetchCountOfApplication>

        public async Task<List<FetchDataForVerticalChart>> FetchCountOfApplicationIdForVerticalChartAsync(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                List<FetchDataForVerticalChart> fetchDataForVerticalCharts = new List<FetchDataForVerticalChart>();

                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1);

                string regionCode = getAllApplicationIdForReport.region_code!;
                string districtCode = getAllApplicationIdForReport.district_code!;
                string officeCode = getAllApplicationIdForReport.office_code!;

                Dictionary<string, int> result = new Dictionary<string, int>();

                //var expectedStatusCodes = new Dictionary<int, string>
                //{
                //   { 0, "Partially Submitted/Pending" },
                //    { 10, "Application is submitted to EPCIS" },
                //    { 11, "Truti Patra is generated" },
                //    { 12, "Application is rejected" },
                //    { 13, "Nikali Patra is generated" },
                //    { 14, "Notice 9 is generated" },
                //    { 15, "Inward Number Error" }
                //};

                var statusCodes = new List<int> { 10, 11, 12, 13, 14 };


                IQueryable<ApplicationDTL> query = context.applicationDTL
                    .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate && statusCodes.Contains(s.status));

                var mutationTypes = new List<string> { "03", "04", "09", "06", "01" };

                // All regions
                if (regionCode == "0" && districtCode == "0" && officeCode == "0")
                {
                    var regionData = await GetRegion(_logger);
                    var parts = regionData.Split('$');
                    if (parts.Length != 2 || !int.TryParse(parts[1], out int statusCode) || statusCode != 200)
                        return null;
                    var regionList = JsonConvert.DeserializeObject<List<EPCIRegion>>(parts[0]);

                    if (regionList != null)
                    {
                        foreach (EPCIRegion region in regionList)
                        {
                            if (Convert.ToInt32(region.region_code) != 7)
                            {
                                var districtsData = await GetDistrictByRegion(Convert.ToInt32(region.region_code), _logger);
                                var districtparts = districtsData.Split('$');
                                if (districtparts.Length != 2 || !int.TryParse(districtparts[1], out int dstatusCode) || dstatusCode != 200)
                                    return null;

                                var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(districtparts[0]);
                                var districtCodes = districtList!
                                .Select(d =>
                                {
                                    int code = d.district_code;
                                    return code <= 9 ? "0" + code.ToString() : code.ToString();
                                })
                                .ToList();
                                //query = query.Where(s => districtCodes.Contains(s.district_code!));

                                var grouped = query
                                .Where(a => districtCodes.Contains(a.district_code!) && mutationTypes.Contains(a.mutation_type_code!))
                                .GroupBy(a => new { a.district_code, a.mutation_type_code })
                                .Select(g => new
                                {
                                    District = g.Key.district_code,
                                    MutationType = g.Key.mutation_type_code,
                                    Count = g.Count()
                                })
                                .ToList();

                                var fullData = (from t in districtCodes
                                                from m in mutationTypes
                                                select new
                                                {
                                                    District = t,
                                                    MutationType = m,
                                                    Count = grouped.FirstOrDefault(x => x.District == t && x.MutationType == m)?.Count ?? 0
                                                })
                                .ToList();

                                var result1 = fullData
                                 .GroupBy(x => x.MutationType)
                                 .Select(g => new
                                 {
                                     MutationType = g.Key,
                                     TotalCount = g.Sum(x => x.Count)
                                 })
                                 .ToList();

                                FetchDataForVerticalChart dataForVerticalChart = new FetchDataForVerticalChart();
                                dataForVerticalChart.category = region.region_name;
                                if (result1 != null)
                                {
                                    dataForVerticalChart.data = result1.Select(r => r.TotalCount).ToArray();
                                    fetchDataForVerticalCharts.Add(dataForVerticalChart);
                                }
                            }
                        }
                    }
                }

                // Specific region (with all districts & offices)
                if (regionCode != "0" && districtCode == "0" && officeCode == "0")
                {
                    var districtData = await GetDistrictByRegion(Convert.ToInt32(regionCode), _logger);
                    var parts = districtData.Split('$');
                    if (parts.Length != 2 || !int.TryParse(parts[1], out int statusCode) || statusCode != 200)
                        return null;
                    var districtList = JsonConvert.DeserializeObject<List<EPCIDistrictByRegionList>>(parts[0]);

                    //if (districtList != null)
                    //{
                    //    foreach (EPCIDistrictByRegionList item in districtList)
                    //    {
                    //        var talukaData = await getOfficeByDistrict(item.district_code.ToString(), _logger);

                    //        if (!(Convert.ToInt32(talukaData.Split("|")[1]) >= 200 && Convert.ToInt32(talukaData.Split("|")[1]) <= 299))
                    //            return null;

                    //        var talukaList = JsonConvert.DeserializeObject<List<OfficeByDist>>(talukaData.Split("|")[0]);
                    //        var talukaCodes = talukaList!
                    //        .Select(d =>
                    //        {
                    //            string code = d.office_code!;
                    //            return code.ToString();
                    //        })
                    //        .ToList();
                    //        //query = query.Where(s => talukaCodes.Contains(s.office_code!));


                    //        var grouped = query
                    //        .Where(a => talukaCodes.Contains(a.office_code!) && mutationTypes.Contains(a.mutation_type_code!))
                    //        .GroupBy(a => new { a.office_code, a.mutation_type_code })
                    //        .Select(g => new
                    //        {
                    //            Taluka = g.Key.office_code,
                    //            MutationType = g.Key.mutation_type_code,
                    //            Count = g.Count()
                    //        })
                    //        .ToList();

                    //        var fullData = (from t in talukaCodes
                    //                        from m in mutationTypes
                    //                        select new
                    //                        {
                    //                            Taluka = t,
                    //                            MutationType = m,
                    //                            Count = grouped.FirstOrDefault(x => x.Taluka == t && x.MutationType == m)?.Count ?? 0
                    //                        })
                    //        .ToList();


                    //        var result1 = fullData
                    //         .GroupBy(x => x.MutationType)
                    //         .Select(g => new
                    //         {
                    //             MutationType = g.Key,
                    //             TotalCount = g.Sum(x => x.Count)
                    //         })
                    //         .ToList();

                    //        FetchDataForVerticalChart dataForVerticalChart = new FetchDataForVerticalChart();
                    //        dataForVerticalChart.category = item.district_name;
                    //        if (result1 != null)
                    //        {
                    //            dataForVerticalChart.data = result1.Select(r => r.TotalCount).ToArray();
                    //            fetchDataForVerticalCharts.Add(dataForVerticalChart);
                    //        }
                    //    }
                    //}

                    if (districtList != null)
                    {
                        foreach (EPCIDistrictByRegionList item in districtList)
                        {
                            var districtCodes = districtList!.Where(x => x.district_code == item.district_code)
                            .Select(d =>
                            {
                                int code = d.district_code!;
                                if (code.ToString().Length == 1)
                                {
                                    return "0" + code.ToString();
                                }
                                else
                                {
                                    return code.ToString();
                                }
                            })
                            .ToList();


                            var grouped = query
                            .Where(a => districtCodes.Contains(a.district_code!) && mutationTypes.Contains(a.mutation_type_code!))
                            .GroupBy(a => new { a.district_code, a.mutation_type_code })
                            .Select(g => new
                            {
                                District = g.Key.district_code,
                                MutationType = g.Key.mutation_type_code,
                                Count = g.Count()
                            })
                            .ToList();

                            var fullData = (from t in districtCodes
                                            from m in mutationTypes
                                            select new
                                            {
                                                District = t,
                                                MutationType = m,
                                                Count = grouped.FirstOrDefault(x => x.District == t && x.MutationType == m)?.Count ?? 0
                                            })
                            .ToList();


                            var result1 = fullData
                             .GroupBy(x => x.MutationType)
                             .Select(g => new
                             {
                                 MutationType = g.Key,
                                 TotalCount = g.Sum(x => x.Count)
                             })
                             .ToList();

                            FetchDataForVerticalChart dataForVerticalChart = new FetchDataForVerticalChart();
                            dataForVerticalChart.category = item.district_name;
                            if (result1 != null)
                            {
                                dataForVerticalChart.data = result1.Select(r => r.TotalCount).ToArray();
                                fetchDataForVerticalCharts.Add(dataForVerticalChart);
                            }
                        }
                    }
                }

                //Completed
                if (regionCode != "0" && districtCode != "0" && officeCode == "0")
                {
                    var talukaData = await getOfficeByDistrict(districtCode, _logger);
                    if (!(Convert.ToInt32(talukaData.Split("|")[1]) >= 200 && Convert.ToInt32(talukaData.Split("|")[1]) <= 299))
                        return null;
                    var talukaList = JsonConvert.DeserializeObject<List<OfficeByDist>>(talukaData.Split("|")[0]);


                    //List<OfficeByDist> talukaList = new List<OfficeByDist>();
                    //OfficeByDist talukaData = new OfficeByDist();
                    //talukaData.office_code = "2516";
                    //talukaData.office_name = "नगर भूमापन अधिकारी, क्र.२ पुणे";
                    //talukaList.Add(talukaData);
                    if (talukaList != null)
                    {
                        foreach (OfficeByDist item in talukaList)
                        {
                            var talukaCodes = talukaList!.Where(x => x.office_code == item.office_code)
                            .Select(d =>
                            {
                                string code = d.office_code!;
                                return code.ToString();
                            })
                            .ToList();


                            var grouped = query
                            .Where(a => talukaCodes.Contains(a.office_code!) && mutationTypes.Contains(a.mutation_type_code!))
                            .GroupBy(a => new { a.office_code, a.mutation_type_code })
                            .Select(g => new
                            {
                                Taluka = g.Key.office_code,
                                MutationType = g.Key.mutation_type_code,
                                Count = g.Count()
                            })
                            .ToList();

                            var fullData = (from t in talukaCodes
                                            from m in mutationTypes
                                            select new
                                            {
                                                Taluka = t,
                                                MutationType = m,
                                                Count = grouped.FirstOrDefault(x => x.Taluka == t && x.MutationType == m)?.Count ?? 0
                                            })
                            .ToList();


                            var result1 = fullData
                             .GroupBy(x => x.MutationType)
                             .Select(g => new
                             {
                                 MutationType = g.Key,
                                 TotalCount = g.Sum(x => x.Count)
                             })
                             .ToList();

                            FetchDataForVerticalChart dataForVerticalChart = new FetchDataForVerticalChart();
                            dataForVerticalChart.category = item.office_name;
                            if (result1 != null)
                            {
                                dataForVerticalChart.data = result1.Select(r => r.TotalCount).ToArray();
                                fetchDataForVerticalCharts.Add(dataForVerticalChart);
                            }
                        }
                    }
                }

                //Completed
                if (regionCode != "0" && districtCode != "0" && officeCode != "0")
                {
                    if (Convert.ToInt32(districtCode) <= 9)
                    {
                        districtCode = "0" + districtCode;
                    }
                    //query = query.Where(s => s.district_code == districtCode && s.office_code == officeCode && s.status == 10);

                    var talukaData = await getOfficeByDistrict(districtCode, _logger);
                    if (!(Convert.ToInt32(talukaData.Split("|")[1]) >= 200 && Convert.ToInt32(talukaData.Split("|")[1]) <= 299))
                        return null;
                    var talukaList = JsonConvert.DeserializeObject<List<OfficeByDist>>(talukaData.Split("|")[0]);

                    //List<OfficeByDist> talukaList = new List<OfficeByDist>();
                    //OfficeByDist talukaData = new OfficeByDist();
                    //talukaData.office_code = "2516";
                    //talukaData.office_name = "नगर भूमापन अधिकारी, क्र.२ पुणे";
                    //talukaList.Add(talukaData);

                    var offices = talukaList!.Where(x => x.office_code == officeCode).ToList();

                    if (offices != null)
                    {
                        foreach (OfficeByDist item in offices)
                        {
                            var talukaCodes = offices!
                            .Select(d =>
                            {
                                string code = d.office_code!;
                                return code.ToString();
                            })
                            .ToList();

                            var grouped = query
                            .Where(a => talukaCodes.Contains(a.office_code!) && mutationTypes.Contains(a.mutation_type_code!))
                            .GroupBy(a => new { a.office_code, a.mutation_type_code })
                            .Select(g => new
                            {
                                Taluka = g.Key.office_code,
                                MutationType = g.Key.mutation_type_code,
                                Count = g.Count()
                            })
                            .ToList();

                            var fullData = (from t in talukaCodes
                                            from m in mutationTypes
                                            select new
                                            {
                                                Taluka = t,
                                                MutationType = m,
                                                Count = grouped.FirstOrDefault(x => x.Taluka == t && x.MutationType == m)?.Count ?? 0
                                            })
                            .ToList();

                            var result1 = fullData
                            .GroupBy(x => x.MutationType)
                            .Select(g => new
                            {
                                MutationType = g.Key,
                                TotalCount = g.Sum(x => x.Count)
                            })
                            .ToList();

                            FetchDataForVerticalChart dataForVerticalChart = new FetchDataForVerticalChart();
                            dataForVerticalChart.category = item.office_name;
                            if (result1 != null)
                            {
                                dataForVerticalChart.data = result1.Select(r => r.TotalCount).ToArray();
                                fetchDataForVerticalCharts.Add(dataForVerticalChart);
                            }
                        }
                    }
                }
                return fetchDataForVerticalCharts;
            }
            catch (Exception ex)
            {
                throw new HandleException($"Fetch failed: {ex.Message}");
            }
        }

        public async Task<string> validateMultipleMutationApplications(RequestvalidateMultipleMutationApplications body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("validateMultipleMutationApplications", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var records = System.Text.Json.JsonSerializer.Deserialize<List<ResponsevalidateMultipleMutationApplications>>(response.Split("|")[0]);
                    //var responsedata = JsonConvert.DeserializeObject<List<ResponsevalidateMultipleMutationApplications>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(records) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }
            }
            else return response;
            //return response;
        }

        // 19 Jan 2026
        public async Task<string> reasonForOwnerNameChange(ILogger _logger)
        {
            try
            {
                string query = string.Empty;
                List<EPCISReasonForOwnerNameChangeList> dataList = new List<EPCISReasonForOwnerNameChangeList>();
                var command = context.Database.GetDbConnection().CreateCommand();
                context.Database.OpenConnection();
                command.CommandText = "SELECT name_change_by_code,name_change_by_description FROM epcis.reason_for_owner_name WHERE updated_flag='FALSE'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataList.Add(new EPCISReasonForOwnerNameChangeList()
                        {
                            name_change_by_code = Convert.ToInt32(reader["name_change_by_code"].ToString()),
                            name_change_by_description = reader["name_change_by_description"].ToString()
                        });
                    }
                    reader.Close();
                }
                context.Database.CloseConnection();
                if (dataList.Count > 0)
                {
                    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                }
                else
                {
                    string response = await SendRequestAsync("reasonForOwnerNameChange", HttpMethod.Post, _logger);
                    if (response.Split("|")[1] == "200")
                    {
                        if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                        {
                            var reasonList = System.Text.Json.JsonSerializer.Deserialize<List<EPCISReasonForOwnerNameChangeList>>(response.Split("|")[0]);
                            return JsonConvert.SerializeObject(reasonList) + "|" + response.Split("|")[1];
                        }
                        else
                        {
                            return "Data List is Empty" + "|" + response.Split("|")[1];
                        }
                    }
                    else return response;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public async Task<string> entryDetailsOfRegisteredMutation(EPCISentryDetailsOfRegisteredMutationRequestData body, ILogger _logger)
        {
            string response = await SendRequestAsync("entryDetailsOfRegisteredMutation", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var responseData = System.Text.Json.JsonSerializer.Deserialize<List<EPCISentryDetailsOfRegisteredMutationResponseData>>(response.Split("|")[0].ToString());
                return JsonConvert.SerializeObject(responseData) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;
        }

        // 23 Jan 2026
        public async Task<string> orderGivenByAuthorityNames(ILogger _logger)
        {
            try
            {
                //string query = string.Empty;
                //List<EPICDistrict> dataList = new List<EPICDistrict>();
                //var command = context.Database.GetDbConnection().CreateCommand();
                //context.Database.OpenConnection();
                //command.CommandText = "SELECT district_code,district_name FROM epcis.district WHERE updated_flag='FALSE'";
                //using (var reader = command.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        dataList.Add(new EPICDistrict()
                //        {
                //            district_code = reader["district_code"].ToString(),
                //            district_name = reader["district_name"].ToString()
                //        });
                //    }
                //    reader.Close();
                //}
                //context.Database.CloseConnection();
                //if (dataList.Count > 0)
                //{
                //    return JsonConvert.SerializeObject(dataList) + "|" + "200";
                //}
                //else
                //{
                string response = await SendRequestAsync("orderGivenByAuthorityNames", HttpMethod.Post, _logger);
                if (response.Split("|")[1] == "200")
                {
                    if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                    {
                        var districts = System.Text.Json.JsonSerializer.Deserialize<List<EPCISOrderGivenByAuthorityNamesResponseData>>(response.Split("|")[0]);
                        return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                    }
                    else
                    {
                        return "Data List is Empty" + "|" + response.Split("|")[1];
                    }
                }
                else return response;
                //
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public async Task<string> getTenureList(string district_code, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("district_code", district_code);
            string response = await SendRequestAsync("getTenureList", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
                {
                    var districts = JsonConvert.DeserializeObject<List<EPCISGetTenureList>>(response.Split("|")[0]);
                    return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
                }
                else
                {
                    return "Data List is Empty" + "|" + response.Split("|")[1];
                }

            }
            else return response;
        }

        public async Task<string> getTenure(EPCISgetTenureRequestData body, ILogger _logger)
        {
            string response = await SendRequestAsync("entryDetailsOfRegisteredMutation", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var responseData = System.Text.Json.JsonSerializer.Deserialize<List<EPCISGetTenureList>>(response.Split("|")[0].ToString());
                return JsonConvert.SerializeObject(responseData) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;
        }
    }
}
