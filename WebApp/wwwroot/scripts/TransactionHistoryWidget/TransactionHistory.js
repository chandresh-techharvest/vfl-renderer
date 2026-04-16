// Add this outside $(function() { ... })
const tableStates = {};  // { tableId: { cursors: [] } }

function getTableState(tableId) {
    if (!tableStates[tableId]) tableStates[tableId] = { lastCursor: null };
    return tableStates[tableId];
}

function resetTableState(tableId) {
    if (tableStates[tableId]) {
        tableStates[tableId].lastCursor = null;
    }
}

$(function () {

    // Helper: Build GraphQL query and variables for DataTables server-side processing
    function buildGraphQLQuery(transactionType, dtParams, extraFilters, tableState) {
        const first = dtParams.length;
        const after = tableState.lastCursor || null;

        // Build the 'where' object according to the required GraphQL structure
        let where = {};
        const serviceTypeMap = {
            'DirectTopUp': 'DirectTopUp',
            'WebTopUp': 'WebTopUp',
            'PlanActivation': 'PlanActivation',
            'PurchasePlan': 'PurchasePlan',
            'PrepayGift': 'PrepayGift'
        };

        // Collect all filter conditions
        let conditions = [];

        // Always add serviceTypeName filter
        const serviceTypeName = serviceTypeMap[transactionType] || 'DirectTopUp';
        conditions.push({
            serviceTypeName: { eq: serviceTypeName }
        });

        // Add date range filter if both dates are present
        if (extraFilters?.fromdate && extraFilters?.todate) {
            conditions.push({
                and: [
                    {
                        serviceDate: {
                            gte: new Date(extraFilters.fromdate).toISOString()
                        }
                    },
                    {
                        serviceDate: {
                            lte: new Date(extraFilters.todate).toISOString()
                        }
                    }
                ]
            });
        } else {
            // Add individual date filters if only one is present
            if (extraFilters?.fromdate) {
                conditions.push({
                    serviceDate: {
                        gte: new Date(extraFilters.fromdate).toISOString()
                    }
                });
            }
            if (extraFilters?.todate) {
                conditions.push({
                    serviceDate: {
                        lte: new Date(extraFilters.todate).toISOString()
                    }
                });
            }
        }

        // Add completion status filter
        if (extraFilters?.completionStatusName) {
            conditions.push({
                completionStatusName: { eq: extraFilters.completionStatusName }
            });
        }

        // Add number search filter (using 'or' array as per sample)
        if (extraFilters?.number) {
            // If we have other conditions, we need to structure it properly
            if (conditions.length > 0) {
                // Create a separate where clause that combines existing conditions with number search
                where = {
                    and: [
                        ...conditions,
                        {
                            or: [{ number: { contains: extraFilters.number } }]
                        }
                    ]
                };
            } else {
                // Only number search
                where = {
                    or: [{ number: { contains: extraFilters.number } }]
                };
            }
        } else {
            // No number search, use conditions as-is
            if (conditions.length === 1) {
                // Single condition, no need for 'and' wrapper
                where = conditions[0];
            } else if (conditions.length > 1) {
                // Multiple conditions, wrap in 'and'
                where = {
                    and: conditions
                };
            }
        }

        // Add DataTables global search (if supported)
        if (dtParams.search?.value) {
            if (Object.keys(where).length > 0) {
                // Combine existing where with global search
                where = {
                    and: [
                        where,
                        { globalSearch: { contains: dtParams.search.value } }
                    ]
                };
            } else {
                where = { globalSearch: { contains: dtParams.search.value } };
            }
        }

        // Sorting: DataTables order to GraphQL order array
        let order = [{ serviceDate: "DESC" }];
        if (dtParams.order && dtParams.order.length > 0) {
            order = dtParams.order.map(ord => {
                const colIdx = ord.column;
                const colName = dtParams.columns[colIdx].data;
                let sortObj = {};
                sortObj[colName] = ord.dir.toUpperCase();
                return sortObj;
            });
        }

        // Convert to GraphQL syntax
        const orderStr = order.map(o => {
            const field = Object.keys(o)[0];
            const dir = o[field];
            return `{${field}: ${dir}}`;
        }).join(', ');

        // Map transactionType to GraphQL root field and node fields
        let rootField, nodeFields, filterType;
        switch (transactionType) {
            case 'DirectTopUp':
                rootField = 'allDirectTopUps';
                filterType = 'PrePurchasesDtoFilterInput';
                nodeFields = 'serviceDate\r\n                    number\r\n                    reference\r\n                    pin\r\n                    completionStatusName\r\n';
                break;
            case 'WebTopUp':
                rootField = 'allTransactions';
                filterType = 'TransactionDtoFilterInput';
                nodeFields = 'serviceDate\r\n                    number\r\n                    reference\r\n                    amount\r\n                    paymentMethodName\r\n                    completionStatusName\r\n';
                break;
            case 'PurchasePlan':
                rootField = 'allTransactions';
                filterType = 'TransactionDtoFilterInput';
                nodeFields = 'serviceDate\r\n                    purchased_Plan_Name\r\n                    number\r\n                    reference\r\n                    amount\r\n                    paymentMethodName\r\n                    completionStatusName\r\n';
                break;
            case 'PrepayGift':
                rootField = 'allSubscriptions';
                filterType = 'SubscriptionDtoFilterInput';
                nodeFields = 'number\r\n                gifterNumber\r\n                plan_Name\r\n                serviceDate\r\n                amount\r\n              processTypeName\r\n                completionStatusName\r\n';
                break;
            case 'PlanActivation':
            default:
                rootField = 'allSubscriptions';
                filterType = 'SubscriptionDtoFilterInput';
                nodeFields = 'number\r\n                    plan_Name\r\n                    serviceDate\r\n                    amount\r\n                    processTypeName\r\n                    completionStatusName\r\n';
        }

        // GraphQL query string (matches your provided schema)
        const query = 'query(\r\n$first: Int!,\r\n $after: String,\r\n $where: ' + filterType + '\r\n) {\r\n                ' + rootField + '(first: $first, after: $after, where: $where, order: ' + orderStr + ') {\r\n                    totalCount\r\n                    edges {\r\n                        cursor\r\n                        node {\r\n                            ' + nodeFields + '                        }\r\n                    }\r\n                    pageInfo {\r\n                        endCursor\r\n                        hasNextPage\r\n                    }\r\n                }\r\n            }';

        // Variables for the query
        const variables = {
            first: first,
            after: after,
            where: Object.keys(where).length > 0 ? where : null
        };

        return { query, variables, rootField };
    }

    // Helper: Get extra filters from UI - Updated to work with date range picker
    function getExtraFilters() {
        const transactionStatus = $("#ddlTransactionStatus").val();
        const number = $("#txtNumber").val();
        //const daterange = $("#daterange").val(); // Assuming this is the date range picker field

        let fromdate = $("#FromDate").val(), todate = $("#ToDate").val();
        

        let filter = {};
        if (transactionStatus && transactionStatus !== '0') filter.completionStatusName = transactionStatus;
        if (number) filter.number = number;
        if (fromdate) filter.fromdate = fromdate;
        if (todate) filter.todate = todate;
        return filter;
    }

    // Rest of your code remains the same...
    const columnsConfig = {
        DirectTopUp: [
            {
                data: 'serviceDate', title: 'Service Date', defaultContent: '',
                render: function (data) {
                    if (data == null) return "";
                    var date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
            },
            { data: 'number', title: 'Number', defaultContent: '' },
            { data: 'reference', title: 'Reference', defaultContent: '' },
            { data: 'pin', title: 'Pin', defaultContent: '' },
            { data: 'completionStatusName', title: 'Completion Status Name', defaultContent: '' }
        ],
        WebTopUp: [
            {
                data: 'serviceDate', title: 'Service Date', defaultContent: '',
                render: function (data) {
                    if (data == null) return "";
                    var date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
            },
            { data: 'number', title: 'Number', defaultContent: '' },
            { data: 'reference', title: 'Reference', defaultContent: '' },
            { data: 'amount', title: 'Amount', defaultContent: '' },
            { data: 'paymentMethodName', title: 'Payment Method Name', defaultContent: '' },
            { data: 'completionStatusName', title: 'Completion Status Name', defaultContent: '' }
        ],
        PlanActivation: [
            {
                data: 'serviceDate', title: 'Service Date', defaultContent: '',
                render: function (data) {
                    if (data == null) return "";
                    var date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
            },
            { data: 'number', title: 'Number', defaultContent: '' },
            { data: 'plan_Name', title: 'Plan Name', defaultContent: '' },
            { data: 'amount', title: 'Amount', defaultContent: '' },
            { data: 'processTypeName', title: 'Process Type Name', defaultContent: '' },
            { data: 'completionStatusName', title: 'Completion Status Name', defaultContent: '' }
        ],
        PurchasePlan: [
            {
                data: 'serviceDate', title: 'Service Date', defaultContent: '',
                render: function (data) {
                    if (data == null) return "";
                    var date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
            },
            { data: 'number', title: 'Number', defaultContent: '' },
            { data: 'reference', title: 'Reference', defaultContent: '' },
            { data: 'purchased_Plan_Name', title: 'Purchased Plan Name', defaultContent: '' },
            { data: 'amount', title: 'Amount', defaultContent: '' },
            { data: 'paymentMethodName', title: 'Payment Method Name', defaultContent: '' },
            { data: 'completionStatusName', title: 'Completion Status Name', defaultContent: '' }
        ],
        PrepayGift: [
            {
                data: 'serviceDate', title: 'Service Date', defaultContent: '',
                render: function (data) {
                    if (data == null) return "";
                    var date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                }
            },
            { data: 'number', title: 'Number', defaultContent: '' },
            { data: 'gifterNumber', title: 'Gifter Number', defaultContent: '' },
            { data: 'plan_Name', title: 'Plan Name', defaultContent: '' },
            { data: 'amount', title: 'Amount', defaultContent: '' },
            { data: 'processTypeName', title: 'Process Type Name', defaultContent: '' },
            { data: 'completionStatusName', title: 'Completion Status Name', defaultContent: '' }
        ]
   };

    // DataTables initialization for each transaction type table
    function initServerSideTable(tableId, transactionType, columns, url) {
        if (!$.fn.dataTable.isDataTable(tableId)) {
            $(tableId).show();

            const table = $(tableId).DataTable({
                serverSide: true,
                processing: true,
                order: [[0, 'desc']],
                responsive: true,
                destroy: true,
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                language: {
                    searchPlaceholder: 'Search...',
                    sSearch: '',
                },
                columns: columns,
                pagingType: "simple",
                ajax: function (data, callback, settings) {
                    const tableId = settings.sTableId || settings.nTable.id;
                    const tableState = getTableState(tableId);

                    // Reset on search/sort/filter (page 0)
                    const hasSearchOrSort = data.search.value ||
                        (data.order && data.order[0] && data.order[0].column !== 0);
                    if (hasSearchOrSort) {
                        tableState.lastCursor = null;
                    }

                    const extraFilters = getExtraFilters();
                    const gql = buildGraphQLQuery(transactionType, data, extraFilters, tableState);

                    console.log('GraphQL Variables:', JSON.stringify(gql.variables, null, 2));

                    $.ajax({
                        url: url,
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({ query: gql.query, variables: gql.variables }),
                        success: function (resp) {
                            const root = resp.data && resp.data[gql.rootField];
                            const nodes = root && root.edges ? root.edges.map(e => e.node) : [];
                            const totalCount = root && typeof root.totalCount === 'number' ? root.totalCount : 0;

                            // Store LAST cursor for next sequential page
                            if (root && root.pageInfo && root.pageInfo.endCursor) {
                                tableState.lastCursor = root.pageInfo.endCursor;
                            }

                            callback({
                                draw: data.draw,
                                recordsTotal: totalCount,
                                recordsFiltered: totalCount,
                                data: nodes
                            });
                        },
                        error: function (xhr, status, error) {
                            tableState.lastCursor = null;
                            callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
                            console.error("GraphQL DataTables error:", error, xhr.responseText);
                        }
                    });
                }
            });
        }
    }

    // Hide all tables and destroy existing DataTables
    function resetTables() {
        const tableIds = ['#DirectTopUpDataTable', '#WebTopUpDataTable', '#PlanActivationDataTable', '#PurchasePlanDataTable', '#PrepayGiftDataTable'];
        tableIds.forEach(function (id) {
            const tableId = id.replace('#', '');
            if ($.fn.dataTable.isDataTable(id)) {
                $(id).DataTable().destroy();
                $(id).empty();
            }
            $(id).hide();
            delete tableStates[tableId];
        });
    }

    $("#btnSearch").on("click", function (e) {
        var filters = getExtraFilters();
        let url = '';
        var transactionType = $("#ddlTransactionType").val();
        resetTables();

        switch (transactionType) {
            case 'DirectTopUp':
                $("#DirectTopUpDataTable").show();
                url = '/api/TransactionHistory/GetDirectTopUpHistory';
                initServerSideTable('#DirectTopUpDataTable', 'DirectTopUp', columnsConfig.DirectTopUp, url);
                break;
            case 'WebTopUp':
                $("#WebTopUpDataTable").show();
                url = '/api/TransactionHistory/GetWebTopUpHistory';
                initServerSideTable('#WebTopUpDataTable', 'WebTopUp', columnsConfig.WebTopUp, url);
                break;
            case 'PlanActivation':
                $("#PlanActivationDataTable").show();
                url = '/api/TransactionHistory/GetPlanActivationHistory';
                initServerSideTable('#PlanActivationDataTable', 'PlanActivation', columnsConfig.PlanActivation, url);
                break;
            case 'PurchasePlan':
                $("#PurchasePlanDataTable").show();
                url = '/api/TransactionHistory/GetPurchasePlanHistory';
                initServerSideTable('#PurchasePlanDataTable', 'PurchasePlan', columnsConfig.PurchasePlan, url);
                break;
            case 'PrepayGift':
                $("#PrepayGiftDataTable").show();
                url = '/api/TransactionHistory/GetPrepayGiftHistory';
                initServerSideTable('#PrepayGiftDataTable', 'PrepayGift', columnsConfig.PrepayGift, url);
                break;
        }
    });
});
