using static System.Console;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Collections;

namespace Calculate{
    public class Program{
        static void Main(){

            CalcController control = new CalcController();
            control.createNewCalculatorForm();

            // myFunction();

            // int myFunction(){
            //     int[] anArray = new int[5] { 78, 83, 23, 34, 56 };
            //     bool f = false;
            //     int c = -1;
            //     int i = 0;
            //     int b = 75;
            //     WriteLine(anArray.Count());

            //     while (!f & i < anArray.Count()){
            //         if (anArray[i] == b){
            //             f = true;
            //             c = i;
            //         }
            //         else {
            //         Console.WriteLine(i);
            //         i++;}
            //     }
            //     return c;
            // }
        }
    }
}