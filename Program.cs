using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x264_qp_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilename = "";
            if (args.Length == 0)
            {
                inputFilename = "ColorCube.mov";
                Console.WriteLine("Defaulting to ColorCube.mov, use 1st argument for other name");
            }
            else
            {
                inputFilename = args[1];
            }

            string[] profileArr = { "baseline", "main", "high", "high10", "high422", "high444" };
            string[] presetArr = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
            string[] pix_fmtArr = { "yuv420p", "yuv422p", "yuv444p", "yuv420p10le", "yuv422p10le", "yuv444p10le" };

            string inputFilenameNoExt = inputFilename.Substring(0, (inputFilename.Length - inputFilename.LastIndexOf(".") + 1));

            string ffmpeg = "ffmpeg"; // Normal usage. I have renamed the executeable to keep track of the specific version
            //string ffmpeg = "ffmpeg-repro";
            string ff = ffmpeg + " -i " + inputFilename + " -c:v libx264 ";
            string profileStr = "-profile:v ";
            string presetStr = "-preset ";
            string QPStr = "-qp ";
            string pix_fmtStr = "-pix_fmt ";

            int numberOfEncodes = 2;

            List<string> writeStrList = new List<string>();
            String outputFilename = "";
            
            int QP_max = 85;

            for (int profileInt = 0; profileInt < profileArr.Length; profileInt++)
            {
                for (int presetInt = 0; presetInt < presetArr.Length; presetInt++)
                {
                    for (int QPInt = 0; QPInt < QP_max; QPInt++) // qp is defined from 0 to 69 i 8 bit, 0-81 in 10 bit. I only expand that range for reproducibility reasons
                        // 
                    {
                        for (int pix_fmtInt = 0; pix_fmtInt < pix_fmtArr.Length; pix_fmtInt++)
                        {
                            for (int encodeInt = 0; encodeInt < (numberOfEncodes); encodeInt++)
                            {
                                // add profile + format, preset and crf to FFmpeg comand, add encode number to output filename

                                //// In baseline, main and high only yuv420p is supported, no 10 bit format or 422 or 444 subsampling is supported
                                //if ((profileInt == 0 || profileInt == 1 || profileInt == 2) && (pix_fmtInt != 0 || CRFInt == 0))
                                //{
                                //    break;
                                //}
                                //// in high10 only up to 420p10le is supported
                                //if (profileInt == 3 && (pix_fmtInt == 1 || pix_fmtInt == 2 || pix_fmtInt == 4 || pix_fmtInt == 5 || CRFInt == 0))
                                //{
                                //    break;
                                //}
                                //// high422 (should be named high10422, as it's high10 + 422 subsampling support
                                //if (profileInt == 4 && (pix_fmtInt == 2 || pix_fmtInt == 5 || CRFInt == 0))
                                //{
                                //    break;
                                //}
                                //// high444, again high10444, supports everything

                                string QPIntStr = QPInt.ToString("D2");
                                string encodeIntStr = encodeInt.ToString("D2");



                                string FFCmd = ff + profileStr + profileArr[profileInt] + " " + presetStr + presetArr[presetInt] + " " + QPStr + QPInt + " " + pix_fmtStr + pix_fmtArr[pix_fmtInt];

                                outputFilename = inputFilenameNoExt + "_" + profileInt + profileArr[profileInt] + "_" + presetInt + presetArr[presetInt] + "_QP_" + QPIntStr + "_" + pix_fmtInt + pix_fmtArr[pix_fmtInt] + "_" + encodeIntStr + ".mov";

                                writeStrList.Add(FFCmd + " " + outputFilename);
                            }

                        }


                    }

                    File.WriteAllLines("0-" + profileInt + profileArr[profileInt] + "-" + presetInt + presetArr[presetInt] + ".bat", writeStrList);
                    writeStrList.Clear();
                }

            } // last loop done


        }
    }
}
