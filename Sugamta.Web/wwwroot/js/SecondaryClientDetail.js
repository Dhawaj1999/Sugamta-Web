var dataTable;

function loadDataTable1() {
    dataTable = $('#tblData1').DataTable({
        "ajax": { url: '/secondaryclient/getall1' },
        "columns": [
            { data: 'secondaryClientEmail', "width": "20%" },
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
                data: 'secondaryClientEmail',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <button type="button" onclick="RenderSecondaryClientEditView('${data}');" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</button>               
                     <button type="button" onClick="DeletesecondaryClient('${data}');" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</button>
                    </div>`;
                },
                "width": "25%"
            }
        ]
    });
}
function DeletesecondaryClient(email) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/secondaryclient/delete',
                type: 'DELETE',
                data: { email: email },
                success: function (data) {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Secondary Client has been deleted.",
                        icon: "success",
                        didClose: () => {
                            window.location.reload();
                        }
                    });

                    /*window.location.reload();*/
                }
            });
        }
    });
}


function RenderSecondaryClientEditView(email) {
    RenderSecondaryClientEditView(email);
}
