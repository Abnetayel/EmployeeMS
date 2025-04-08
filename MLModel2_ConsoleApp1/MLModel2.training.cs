﻿// This file was auto-generated by ML.NET Model Builder.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using System.Data.SqlClient;
using Microsoft.ML.Data;

namespace MLModel2_ConsoleApp1
{
    public partial class MLModel2
    {
        public const string RetrainConnectionString = @"Data Source=localhost;Initial Catalog=EmployeeDB;Integrated Security=True;Trust Server Certificate=True";
        public const string RetrainCommandString = null;

        /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="outputModelPath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet"</param>
        /// <param name="connectionString">Connection string for databases on-premises or in the cloud.</param>
        /// <param name="commandText">Command string for selecting training data.</param>
        public static void Train(string outputModelPath, string connectionString = RetrainConnectionString, string commandText = RetrainCommandString)
        {
            var mlContext = new MLContext();

            var data = LoadIDataViewFromDatabase(mlContext, connectionString, commandText);
            var model = RetrainModel(mlContext, data);
            SaveModel(mlContext, model, data, outputModelPath);
        }

        /// <summary>
        /// Load an IDataView from a database source.For more information on how to load data, see aka.ms/loaddata.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="connectionString">Connection string for databases on-premises or in the cloud.</param>
        /// <param name="commandText">Command string for selecting training data.</param>
        /// <returns>IDataView with loaded training data.</returns>
        public static IDataView LoadIDataViewFromDatabase(MLContext mlContext, string connectionString, string commandText)
        {
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, commandText);

            return loader.Load(dbSource);
        }

        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <param name="modelSavePath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }


        /// <summary>
        /// Retrain model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"Id", @"Id"),new InputOutputColumnPair(@"Age", @"Age")})      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"FullName",outputColumnName:@"FullName"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Gender",outputColumnName:@"Gender"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Skills",outputColumnName:@"Skills"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"EmployementType",outputColumnName:@"EmployementType"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"PhoneNumber",outputColumnName:@"PhoneNumber"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Country",outputColumnName:@"Country"))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"Education",outputColumnName:@"Education"))      
                                    .Append(mlContext.Transforms.Conversion.ConvertType(@"DateOfRegistration", @"DateOfRegistration"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"Id",@"Age",@"FullName",@"Gender",@"Skills",@"EmployementType",@"PhoneNumber",@"Country",@"Education",@"DateOfRegistration"}))      
                                    .Append(mlContext.Regression.Trainers.FastForest(new FastForestRegressionTrainer.Options(){NumberOfTrees=4,NumberOfLeaves=4,FeatureFraction=1F,LabelColumnName=@"Experience",FeatureColumnName=@"Features"}));

            return pipeline;
        }
    }
 }
