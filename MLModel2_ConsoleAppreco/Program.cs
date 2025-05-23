﻿
// This file was auto-generated by ML.NET Model Builder. 

using MLModel2_ConsoleAppreco;

// Create single instance of sample data from first line of dataset for model input
MLModel2.ModelInput sampleData = new MLModel2.ModelInput()
{
    Education = @"Master",
    Skills = @"Backend",
};



Console.WriteLine("Using model to make single prediction -- Comparing actual Salary with predicted Salary from sample data...\n\n");


Console.WriteLine($"Education: {@"Master"}");
Console.WriteLine($"Skills: {@"Backend"}");
Console.WriteLine($"Salary: {95000F}");


// Make a single prediction on the sample data and print results
var predictionResult = MLModel2.Predict(sampleData);
Console.WriteLine($"\n\nPredicted Salary: {predictionResult.Score}\n\n");

Console.WriteLine("=============== End of process, hit any key to finish ===============");
Console.ReadKey();

