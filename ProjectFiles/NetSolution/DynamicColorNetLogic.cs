#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
#endregion


/*
 * Dynamic Color NetLogic: showing the capability of accessing and dynamically changing objects based on value change in optix project
 * 
 * Gauge Coloring: is only accessible through creating new style sheets under Gauge Styles in the DefaultStyleSheet. Each individual circular 
 * guage references this style sheet rather than having color as individual property (like the rectangle). This code accesses the three style sheets 
 * created and sets each individual guage to the color equivalent to the value input.
 * Rectangle Coloring: is an example of directly accessing the rectangle object and changing its specific coloring.
 * 
 * 1/19/2024 written by John Goodwin
 * 
 */
public class DynamicColorNetLogic : BaseNetLogic
{
    private IUAVariable addend1;
    private IUAVariable addend2;
    private RemoteVariableSynchronizer variableSynchronizer;
    Rectangle myRectangle;
    Rectangle myRectangle2;
    CircularGauge myCircularGauge;
    CircularGauge myCircularGauge2;
    StyleSheet myStyleGreen;
    StyleSheet myStyleYellow;
    StyleSheet myStyleRed;
    PeriodicTask periodicTask;
    CircularGauge[] circularGaugeArray;
    Rectangle[] rectangleArray;

    Color red = new Color(0xffff0000);
    Color yellow = new Color(0xffffcc00);
    Color green = new Color(0xff25e45f);

    /*
     * Initializing the value of each of the objects and implementing the variable change synchronizer
     */
    public override void Start()
    { 
        
        myRectangle = Project.Current.Get<Rectangle>("UI/MainWindow/Rectangle1");
        myRectangle2 = Project.Current.Get<Rectangle>("UI/MainWindow/Rectangle2");

        myStyleGreen = Project.Current.Get<StyleSheet>("UI/GreenSheet");
        myStyleYellow = Project.Current.Get<StyleSheet>("UI/YellowSheet");
        myStyleRed= Project.Current.Get<StyleSheet>("UI/RedSheet");

        myCircularGauge = Project.Current.Get<CircularGauge>("UI/MainWindow/CircularGauge1");
        myCircularGauge2 = Project.Current.Get<CircularGauge>("UI/MainWindow/CircularGauge2");

        myRectangle.BorderThickness = 10;
        myRectangle2.BorderThickness = 10;

        circularGaugeArray = new[] {myCircularGauge, myCircularGauge2};
        rectangleArray = new[] { myRectangle, myRectangle2 };

        addend1 = LogicObject.GetVariable("Variable1");
        addend2 = LogicObject.GetVariable("Variable2");
        addend1.VariableChange += circularGuage1;
        addend2.VariableChange += circularGuage2;

        variableSynchronizer = new RemoteVariableSynchronizer();
        variableSynchronizer.Add(addend1);
        variableSynchronizer.Add(addend2);

    }

    public override void Stop()
    {
        addend1.VariableChange -= circularGuage1;
        addend2.VariableChange -= circularGuage2;
    }

    /*
     * On the event that the value of "Variable1" changes, the changeFunc is called on the 
     * circular guage and rectangle that coincides with that value
     */
    private void circularGuage1(object sender, VariableChangeEventArgs e)
    {
        myCircularGauge.Value = LogicObject.GetVariable("Variable1").Value;
        ChangeFunc(myCircularGauge, myRectangle);

        // Function could also be implemented as:
        // circularGaugeArray[0].Value = LogicObject.GetVariable("Variable1").Value;
        //ChangeFunc(circularGaugeArray[0], rectangleArray[0]);
    }

   /*
   * On the event that the value of "Variable2" changes, the changeFunc is called on the 
   * circular guage and rectangle that coincides with that value
   */
    private void circularGuage2(object sender, VariableChangeEventArgs e)
    {
        myCircularGauge2.Value = LogicObject.GetVariable("Variable2").Value;
        ChangeFunc(myCircularGauge2, myRectangle2);

        // Function could also be implemented as:
        // circularGaugeArray[1].Value = LogicObject.GetVariable("Variable2").Value;
        // ChangeFunc(circularGaugeArray[1], rectangleArray[1]);
    }


    /*
     * Based on the guage value, the sheet style is changed for that specific circular guage
     * and the rectangle color can be changed directly through its properties.
     */
    private void ChangeFunc(CircularGauge guageChange, Rectangle rectangleChange)
    {

            if (guageChange.Value < 50)
            {
                guageChange.GaugeStyle = "RedSheet";
                rectangleChange.BorderColor = red;
            }
            else if (guageChange.Value < 75 && guageChange.Value >= 50)
            {
                guageChange.GaugeStyle = "YellowSheet";
                rectangleChange.BorderColor = yellow;
            }
            else
            {
                guageChange.GaugeStyle = "GreenSheet";
                rectangleChange.BorderColor = green;
            }

        
    }

}
