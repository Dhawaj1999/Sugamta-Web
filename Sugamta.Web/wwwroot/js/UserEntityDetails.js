var dataTable;

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/userentity/getall' },
        "columns": [
            { data: 'email', "width": "20%" },
            { data: 'name', "width": "15%" },
            {
                data: 'creationDate',
                "width": "15%",
                "render": function (data) {
                    var formattedDate = new Date(data).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
                    return formattedDate;
                }
            },
            { data: 'roleType', "width": "15%" },
            {
                data: 'email',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <button type="button" onclick="RenderUserEditView('${data}');" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</button>               
                     <button type="button" onClick="Delete('${data}');" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</button>
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(email) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // Perform deletion logic here, you can use the 'email' parameter if needed
            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}

function RenderUserEditView(email) {
    renderUserManagementEditPartialView(email);
}
