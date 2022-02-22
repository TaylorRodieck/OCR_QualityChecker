using GroupDocs.Parser;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace OCR_QA_1
{
    
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            FolderBrowserDialog mainFolder = new FolderBrowserDialog();
            if(mainFolder.ShowDialog() == DialogResult.OK)
            {
                 string folderPath = System.IO.Path.GetFullPath(mainFolder.SelectedPath);
                Console.WriteLine(folderPath);
                 foreach(var pdfFile in Directory.GetFiles(folderPath,"*.pdf"))
                {
                    if (string.IsNullOrEmpty(pdfFile))
                    {
                        throw new ArgumentNullException("pdfFile is Empty");
                    }
                    if (!System.IO.File.Exists(pdfFile))
                    {
                        throw new System.IO.FileNotFoundException("pdfFile Not Found!");
                    }

                    using (FileStream SR = new FileStream(pdfFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(SR);

                        StringBuilder Buf = new StringBuilder();

                        //Use the PdfTextExtractor to get all of the text on a page-by-page basis
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            Buf.AppendLine(PdfTextExtractor.GetTextFromPage(reader, i));
                        }

                        string fileName = System.IO.Path.GetFileName(pdfFile);

                        decimal wordCount = Regex.Matches(Buf.ToString(), "\\S+").Count;
                        string[] wordList = Regex.Split(Buf.ToString(), "[\\s+]");
                        string[] lines = File.ReadAllLines(@"D:\!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Notes\Every_English_Word_Ever.txt");
                        decimal correctWords = 0;
                        foreach(string word in wordList)
                        {
                            string newString = (Regex.Replace(word, @"[^\w\';:]", "")).ToLower();
                            //Console.WriteLine(newString);

                            foreach (string line in lines)
                            {
                                if (line.Contains(newString) == true)
                                {
                                    correctWords++;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        decimal percentCorrect = decimal.Round((correctWords / wordCount), 2);
                        percentCorrect *= 100;
                        string filePath = System.IO.Path.GetFullPath(pdfFile);
                        string acceptableOCRFolderPath = System.IO.Path.Combine(folderPath, "AcceptableOCR");
                        string passableOCRFolderPath = System.IO.Path.Combine(folderPath, "PassableOCR");
                        string badOCRFolderPath = System.IO.Path.Combine(folderPath, "BadOCR");

                        if (Directory.Exists(acceptableOCRFolderPath) != true)
                        {
                            Directory.CreateDirectory(acceptableOCRFolderPath);
                        }
                        else if (Directory.Exists(passableOCRFolderPath) != true)
                        {
                            Directory.CreateDirectory(passableOCRFolderPath);
                        }
                        else if (Directory.Exists(badOCRFolderPath) != true)
                        {
                            Directory.CreateDirectory(badOCRFolderPath);
                        }

                        if (percentCorrect > 90)
                        {
                            string destFile1 = System.IO.Path.Combine(acceptableOCRFolderPath, fileName);
                            File.Move(filePath, destFile1);
                        }
                        else if (percentCorrect <= 89 && percentCorrect >= 50)
                        {
                            string destFile2 = System.IO.Path.Combine(passableOCRFolderPath, fileName);
                            File.Move(filePath, destFile2);
                        }
                        else if (percentCorrect < 50)
                        {
                            string destFile3 = System.IO.Path.Combine(badOCRFolderPath, fileName);
                            File.Move(filePath, destFile3);
                        }

                        Console.WriteLine("There are " + wordCount + " words total within " + fileName);
                        Console.WriteLine("There are " + correctWords + " actual words within " + fileName);
                        Console.WriteLine();
                        Console.WriteLine("Accuracy: " + percentCorrect + "%");
                        Console.WriteLine("----------------------------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                    
                 }
            }
            Console.WriteLine("Press Any Key to [ESC]...");
			Console.ReadKey();
		}
    }
}
