﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Slacker.Helpers;
using Slacker.Helpers.Attributes;
using System.Linq.Expressions;
using System.Data;
using System.Dynamic;
using FastMember;
using System.Threading.Tasks;
using Slacker.Exceptions;

namespace Slacker {

    /// <summary>
    /// Used as a marker for reflection. Cast to IDataService<T> to use.
    /// </summary>
    public interface IDataService { }
    /// <summary>
    /// Represents a Slacker DataService manager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataService<T> : IDataService where T : DataModel, new() {
        #region Insert
        /// <summary>
        /// Perform an insert query using data model
        /// </summary>
        /// <param name="loadGeneratedKeys">Should generated keys (ID fields) be loaded to models on insert.</param>
        void Insert(T model, bool loadGeneratedKeys = true);
        /// <summary>
        /// Perform an insert query using data model(s)
        /// </summary>
        /// <param name="loadGeneratedKeys">Should generated keys (ID fields) be loaded to models on insert.</param>
        void Insert(T[] models, bool loadGeneratedKeys = true);
        /// <summary>
        /// Perform an async insert query using data model
        /// </summary>
        /// <param name="loadGeneratedKeys">Should generated keys (ID fields) be loaded to models on insert.</param>
        Task InsertAsync(T model, bool loadGeneratedKeys = true);
        /// <summary>
        /// Perform an async insert query using data model(s)
        /// </summary>
        /// <param name="loadGeneratedKeys">Should generated keys (ID fields) be loaded to models on insert.</param>
        Task InsertAsync(T[] models, bool loadGeneratedKeys = true);
        #endregion
        #region Select
        /// <summary>
        /// Select all records
        /// </summary>
        /// <returns>IEnumerable<typeparamref name="T"/> results</returns>
        IEnumerable<T> SelectAll();
        /// <summary>
        /// Perform an async select using a default condition (Primary Key) with parameter object
        /// </summary>
        /// <param name="whereParam"></param>
        /// <returns>IEnumerable<typeparamref name="T"/> results</returns>
        IEnumerable<T> Find(object whereParam);
        /// <summary>
        /// Perform an select query with Condition
        /// </summary>
        /// <param name="where">Condition query</param>
        /// <param name="whereParam">Condition parameter</param>
        /// <returns>IEnumerable<typeparamref name="T"/>results</returns>
        IEnumerable<T> Select(string where, object whereParam);
        /// <summary>
        /// Select all records async
        /// </summary>
        /// <returns>IEnumerable<typeparamref name="T"/> results</returns>
        Task<IEnumerable<T>> SelectAllAsync();
        /// <summary>
        /// Perform an async select using a default condition (Primary Key) with parameter object
        /// </summary>
        /// <param name="whereParam"></param>
        /// <returns>IEnumerable<typeparamref name="T"/> results</returns>
        Task<IEnumerable<T>> FindAsync(object whereParam);
        /// <summary>
        /// Perform an async select query with Condition
        /// </summary>
        /// <param name="where">Condition query</param>
        /// <param name="whereParam">Condition parameter</param>
        /// <returns>IEnumerable<typeparamref name="T"/>results</returns>
        Task<IEnumerable<T>> SelectAsync(string where, object whereParam);
        #endregion
        #region Update
        /// Performs an update on data model using default primary key based condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="onlyChanged">Only update changed fields on the model</param>
        void Update(T model, bool updateOnlyChangedProperties = true);
        /// <summary>
        /// Performs an update on object model
        /// </summary>
        /// <param name="model">T or anonymous Object (Uses reflection unless updateFields is set)</param>
        /// <param name="updateFields">The fields to be updated or null for all</param>
        /// <param name="where">The condition or null for all models (Requires UpdateAll Flag)</param>
        /// <param name="whereObj">Provides an additional object for where condition specifically.</param>
        void Update(object model, IEnumerable<string> updateFields = null, string where = null, object whereObj = null);
        /// Performs an async update on data model using default primary key based condition
        /// </summary>
        /// <param name="model"></param>
        /// <param name="onlyChanged">Only update changed fields on the model</param>
        Task UpdateAsync(T model, bool updateOnlyChangedProperties = true);
        /// <summary>
        /// Performs an async update on object model
        /// </summary>
        /// <param name="model">T or anonymous Object (Uses reflection unless updateFields is set)</param>
        /// <param name="updateFields">The fields to be updated or null for all</param>
        /// <param name="where">The condition or null for all models (Requires UpdateAll Flag)</param>
        /// <param name="whereObj">Provides an additional object for where condition specifically.</param>
        Task UpdateAsync(object model, IEnumerable<string> updateFields = null,
            string where = null, object whereObj = null);
        #endregion
        #region Delete
        /// <summary>
        /// Global Delete on Table
        /// </summary>
        void DeleteAll();
        /// <summary>
        /// Delete a model by primary key
        /// </summary>
        void Delete(T model);
        /// <summary>
        /// Delete records based on condition and condition parameter
        /// </summary>
        /// <param name="where">Condition query</param>
        /// <param name="whereParam">Condition parameter</param>
        void Delete(string where, object whereParam);
        /// <summary>
        /// Async Global Delete on Table
        /// </summary>
        Task DeleteAllAsync();
        /// <summary>
        /// Async Delete a model by primary key
        /// </summary>
        Task DeleteAsync(T model);
        /// <summary>
        /// Async Delete records based on condition and condition parameter
        /// </summary>
        /// <param name="where">Condition query</param>
        /// <param name="whereParam">Condition parameter</param>
        Task DeleteAsync(string where, object whereParam);
        #endregion

    }

    public abstract class DataServiceProvider<T> : IDataService<T> where T: DataModel, new() {
        #region Insert
        /// <inheritdoc />
        public void Insert(T model, bool loadGeneratedKeys = true) {
            AsyncHelpers.RunSync(InsertAsync(model, loadGeneratedKeys));
        }
        /// <inheritdoc />
        public void Insert(T[] models, bool loadGeneratedKeys = true) {
            AsyncHelpers.RunSync(InsertAsync(models, loadGeneratedKeys));
        }
        /// <inheritdoc />
        public async Task InsertAsync(T model, bool loadGeneratedKeys = true) {
            await InsertAsync(new[] { model }, loadGeneratedKeys);
        }
        /// <inheritdoc />
        public abstract Task InsertAsync(T[] models, bool loadGeneratedKeys = true);
        #endregion

        #region Select
        /// <inheritdoc />
        public IEnumerable<T> SelectAll() {
            return AsyncHelpers.RunSync(SelectAllAsync());
        }
        /// <inheritdoc />
        public IEnumerable<T> Find(object whereParam) {
            return AsyncHelpers.RunSync(FindAsync(whereParam));
        }
        /// <inheritdoc />
        public IEnumerable<T> Select(string where, object whereParam) {
            return AsyncHelpers.RunSync(SelectAsync(where, whereParam));
        }
        /// <inheritdoc />
        public async Task<IEnumerable<T>> SelectAllAsync() {
            return await SelectAsync("", false);
        }
        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> FindAsync(object whereParam);
        /// <inheritdoc />
        public abstract Task<IEnumerable<T>> SelectAsync(string where, object whereParam);
        #endregion

        #region Update
        /// <inheritdoc />
        public void Update(T model, bool updateOnlyChangedProperties = true) {
            AsyncHelpers.RunSync(UpdateAsync(model, updateOnlyChangedProperties));
        }
        /// <inheritdoc />
        public void Update(object model, IEnumerable<string> updateFields = null,
            string where = null, object whereObj = null) {

            AsyncHelpers.RunSync(UpdateAsync(
                model, updateFields, where, whereObj
            ));
        }
        /// <inheritdoc />
        public async Task UpdateAsync(T model, bool updateOnlyChangedProperties = true) {
            if (updateOnlyChangedProperties) {
                if (model.ChangedProperties.Count < 1) {
                    return;
                }

                await UpdateAsync(model, model.ChangedProperties);
                return;
            }

            await UpdateAsync((object) model);
        }
        /// <inheritdoc />
        public abstract Task UpdateAsync(object model, IEnumerable<string> updateFields = null, 
            string where = null, object whereObj = null);
        #endregion

        #region Delete
        /// <inheritdoc />
        public void DeleteAll() {
            AsyncHelpers.RunSync(DeleteAllAsync());
        }
        /// <inheritdoc />
        public void Delete(T model) {
            AsyncHelpers.RunSync(DeleteAsync(model));
        }
        /// <inheritdoc />
        public void Delete(string where, object whereParam) {
            AsyncHelpers.RunSync(DeleteAsync(where, whereParam));
        }
        /// <inheritdoc />
        public async Task DeleteAllAsync() {
            await DeleteAsync("", null);
        }
        /// <inheritdoc />
        public abstract Task DeleteAsync(T model);
        /// <inheritdoc />
        public abstract Task DeleteAsync(string where, object whereParam);
        #endregion

    }

    public abstract class DataService<T> : DataServiceProvider<T> where T : DataModel, new() {
        
        private static ServiceRegistry _serviceRegistry;
        /// <summary>
        /// Stores all singleton type instances of IDataService
        /// </summary>
        public static ServiceRegistry SERVICE_REGISTRY {
            get {
                if (_serviceRegistry == null) {
                    _serviceRegistry = new ServiceRegistry();
                }
                return _serviceRegistry;
            }
        }


        private TypeAccessor _typeAccessor;
        /// <summary>
        /// FastMember TypeAccessor for T
        /// </summary>
        public TypeAccessor TypeAccessor {
            get {
                if (_typeAccessor == null) {
                    _typeAccessor = TypeAccessor.Create(typeof(T));
                }
                return _typeAccessor;
            }
        }

        private string _queryFieldCols;
        /// <summary>
        /// Returns a pre-generated field string for table fields
        /// </summary>
        public string QueryFieldCols {
            get {
                if (_queryFieldCols == null) {
                    _queryFieldCols = string.Join(",", Fields.Select(
                            field => $@"[{field.TableField}]"
                    ));
                }
                return _queryFieldCols;
            }
        }

        private string _queryNonKeyFieldCols;
        /// <summary>
        /// Returns a pre-generated field string for table non-key fields
        /// </summary>
        public string QueryNonKeyFieldCols {
            get {
                if (_queryNonKeyFieldCols == null) {
                    _queryNonKeyFieldCols = string.Join(",",
                        NonGeneratedFields.Select(
                            field => $@"[{field.TableField}]"
                        )
                    );
                }
                return _queryNonKeyFieldCols;
            }
        }

        private string _queryModelRefs;
        /// <summary>
        /// Returns a pre-generated field string for table fields
        /// </summary>
        public string QueryModelRefs {
            get {
                if (_queryModelRefs == null) {
                    _queryModelRefs = string.Join(",", Fields.Select(
                        field => $"@{field.ModelField}"
                    ));
                }
                return _queryModelRefs;
            }
        }

        private string _queryNonKeyModelRefs;
        /// <summary>
        /// Returns a pre-generated field string for table non-key fields
        /// </summary>
        public string QueryNonKeyGeneratedModelRefs {
            get {
                if (_queryNonKeyModelRefs == null) {
                    _queryNonKeyModelRefs = string.Join(",",
                        NonGeneratedFields.Select(
                            field => $"@{field.ModelField}"
                        )
                    );
                }
                return _queryNonKeyModelRefs;
            }
        }


        private string _querySelects;
        /// <summary>
        /// Returns query selects for model fields
        /// </summary>
        public string QuerySelects {
            get {
                if (_querySelects == null) {
                    _querySelects = string.Join(",",
                        Fields.Select(
                            field => $"[{Alias}].[{field.TableField}] AS [{field.ModelField}]"
                        )
                    );
                }
                return _querySelects;
            }
        }

        private string _queryDefaultUpdateRefs;
        /// <summary>
        /// Returns query updates for all model fields
        /// </summary>
        public string QueryDefaultUpdateRefs {
            get {
                if (_queryDefaultUpdateRefs == null) {
                    _queryDefaultUpdateRefs = string.Join(",",
                        NonGeneratedFields.Select(
                            field => $@"[{Alias}].[{field.TableField}] = @{field.ModelField}"
                        )
                    );
                }
                return _queryDefaultUpdateRefs;
            }
        }


        private bool _tableAttributeSearched;
        public TableAttribute _tableAttribute;
        /// <summary>
        /// Returns the Table Attribute if defined for this table
        /// </summary>
        public TableAttribute TableAttribute {
            get {
                if (!_tableAttributeSearched) {
                    _tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
                    _tableAttributeSearched = true;
                }

                return _tableAttribute;
            }
        }


        private string _table;
        /// <summary>
        /// Database Table
        /// </summary>
        public string Table {
            get {
                if(_table == null) {
                    _table = TableAttribute?.Name ?? typeof(T).Name;
                }
                return _table;
            }
            set {
                _table = value;
            }
        }

        private string _alias;
        /// <summary>
        /// Database Alias
        /// </summary>
        public string Alias {
            get {
                if (_alias == null) {
                    _alias = TableAttribute?.Alias ?? 
                        typeof(T).Name.PadRight(3, '0').Substring(3);
                }
                return _alias;
            }
            set {
                _alias = value;
            }
        }

        
        private string _defaultCondition;
        /// <summary>
        /// Default condition based on Primary Key
        /// </summary>
        public string DefaultCondition {
            get {
                if (_defaultCondition == null) {
                    _defaultCondition = string.Join(" AND ", PrimaryKey.Select(
                        field => $@"([{Alias}].[{field.TableField}] = @{field.ModelField})"
                    ));
                }
                return _defaultCondition;
            }
        }

        private List<DataModelField> _primaryKey;
        /// <summary>
        /// Return Primary Key Fields 
        /// </summary>
        public List<DataModelField> PrimaryKey {
            get {
                if (_primaryKey == null) {
                    _primaryKey = Fields.Where(
                        field => field.IsPrimary
                    ).ToList();
                }
                return _primaryKey;
            }
        }

        private List<DataModelField> _nonGeneratedFields;
        /// <summary>
        /// Return NonKey Fields
        /// </summary>
        public List<DataModelField> NonGeneratedFields {
            get {
                if (_nonGeneratedFields == null) {
                    _nonGeneratedFields = Fields.Where(
                        field => !field.IsGenerated
                    ).ToList();
                }
                return _nonGeneratedFields;
            }
        }



        /// <summary>
        /// Enable this setting to allow this model to use delete
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Enable this setting to allow this model to use delete all
        /// </summary>
        public bool AllowGlobalDelete { get; set; }

        /// <summary>
        /// Enable this setting to allow global updates on this service
        /// </summary>
        public bool AllowGlobalUpdates { get; set; }

        /// <summary>
        /// The SQLConnection for this DataService
        /// </summary>
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// Contains DataField info
        /// </summary>
        public List<DataModelField> Fields { get; protected set; }
        
        /// <summary>
        /// Initializes a new DataService with a given connection
        /// </summary>
        /// <param name="sqlConnection">The SqlConnection</param>
        public DataService(SqlConnection sqlConnection = null) {
            this.Connection = sqlConnection;

            // Register Fields/Properties
            // Potential TODO: Replace with FastMember
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            this.Fields = typeof(T).GetProperties(bindingFlags).Select(
                memberInfo => new DataModelField(memberInfo)
            ).Where(
                dataField => !dataField.IsIgnored
            ).ToList();
            
            // Add DataService as managing service for model (T)
            SERVICE_REGISTRY.Register(typeof(T), this);
        }
        

        #region CRUD Functions
        private string _insertQuery;
        /// <inheritdoc />
        public override async Task InsertAsync(T[] models, bool loadGeneratedKeys = true) {

            // Build Insert Query
            if(_insertQuery == null) { 
                _insertQuery = $@"
                    INSERT INTO [{Table}] ({QueryNonKeyFieldCols}) 
                    VALUES ({QueryNonKeyGeneratedModelRefs});";
            }

            var autoIncField = PrimaryKey.FirstOrDefault(
                pk => pk.FieldAttribute.IsGenerated
            );

            // Do Insert
            foreach (var model in models) {
                
                if (autoIncField == null || !loadGeneratedKeys) {
                    await ExecuteAsync(_insertQuery, model);
                    continue;
                }

                // Update and save generated id to model
                var results = await QueryAsync<int>(
                    _insertQuery + @"SELECT CAST(SCOPE_IDENTITY() as int)",
                    model
                );


                TypeAccessor[model, autoIncField.ModelField] = results.Single();
                model.ChangedProperties.Clear();
            }
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<T>> FindAsync(object whereParam) {
            return await SelectAsync(DefaultCondition, whereParam);
        }
        
        private string _selectQuery;
        /// <inheritdoc />
        public override async Task<IEnumerable<T>> SelectAsync(string where, object whereParam) {
            // Build Query
            if (_selectQuery == null) {
                _selectQuery = $@"SELECT {QuerySelects} FROM [{Table}] [{Alias}]";
            }
            
            // Select with condition
            var results = await QueryAsync<T>(
                _selectQuery + (!string.IsNullOrEmpty(where) ?  " WHERE " + where : ""), 
                whereParam
            );

            // Clear model changes
            results.ToList().ForEach(
                res => res.ChangedProperties.Clear()
            );

            return results;
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(object model, IEnumerable<string> updateFields = null, 
            string where = null, object whereObj = null) {
            
            // Build combined parameter object
            var param = new DynamicParameters(model);
            if(whereObj != null) { 
                param.AddDynamicParams(whereObj);
            }

            // If fields is null and model is anonymous object, lookup properties.
            if (updateFields == null && !(model is T)) {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                updateFields = model.GetType().GetProperties(bindingFlags).Select(
                    m => m.Name
                );
            }

            // If fields is null, use all NonKeyFields else map Fields by tableFieldsToUpdate
            var updateFieldsInfo = updateFields == null ? NonGeneratedFields : Fields.Where(
                field => updateFields.Contains(field.TableField)
            );

            // Blank update set, return
            if (updateFieldsInfo.Count() < 1) {
                return;
            }

            var updateFieldStr = string.Join(", ", (updateFieldsInfo.Select(
                field => $"[{Alias}].[{field.TableField}]=@{field.ModelField}"
            )));

            // Do Update
            string update = $@"
                UPDATE [{Alias}] SET {updateFieldStr}
                FROM [{Table}] [{Alias}]
                WHERE {where ?? DefaultCondition}";

            await ExecuteAsync(update, param);

            if (model is DataModel) {
                ((DataModel) model).ChangedProperties.Clear();
            }
            
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(T model) {
            await DeleteAsync(DefaultCondition, model);
        }

        /// <inheritdoc />
        public override async Task DeleteAsync(string where, object whereParam) {
            string query = $@"DELETE FROM {Table}";

            if (string.IsNullOrEmpty(where)) {
                // Runtime "Sanity" Check
                if (!AllowGlobalDelete) {
                    throw new Exception("DataService.AllowDeleteAll must be enabled to delete all records.");
                }
                // Delete All
                await ExecuteAsync(query);
                return;
            }
            

            // Runtime "Sanity" Check
            if (!AllowDelete) {
                throw new Exception("DataService.AllowDelete must be enabled to delete records");
            }
            // Delete by Condition
            await ExecuteAsync(query + " WHERE " + where, whereParam);
        }

        /// <summary>
        /// Performs a standard Dapper QueryAsync but wraps SqlExceptions with SlackerSqlException
        /// </summary>
        public async Task<IEnumerable<U>> QueryAsync<U>(string query, object parameter = null) {
            try {
                return await Connection.QueryAsync<U>(query, parameter);
            }
            catch (SqlException e) {
                throw new SlackerSqlException(e, query, parameter);
            }
        }

        /// <summary>
        /// Performs a standard Dapper ExecuteAsync but wraps SqlExceptions with SlackerSqlException
        /// </summary>
        public async Task<int> ExecuteAsync(string query, object parameter = null) {
            try {
                return await Connection.ExecuteAsync(query, parameter);
            }
            catch (SqlException e) {
                throw new SlackerSqlException(e, query, parameter);
            }
        }

        #endregion



        #region Helper Methods
        /// <summary>
        /// Converts dataset to DataTable
        /// </summary>
        public static DataTable ToDataTable(IEnumerable<T> data) {
            var dataTable = new DataTable(typeof(T).Name);
            using (var reader = ObjectReader.Create(data)) {
                dataTable.Load(reader);
            }

            return dataTable;
        }
        /// <summary>
        /// Retrieves a registered DataService instance for type
        /// </summary>
        public static DataService<T> GetService() {
            return (DataService<T>) SERVICE_REGISTRY.GetService(typeof(T));
        }
        #endregion

    }
}
