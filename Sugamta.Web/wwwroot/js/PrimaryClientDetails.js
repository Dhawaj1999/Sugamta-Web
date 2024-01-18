var dataTable;

function loadDataClientTable() {
    dataTable = $('#primaryClientTable').DataTable({
        "ajax": { url: '/primaryclient/GetAllPrimaryClient' },
        "columns": [
            { data: 'primaryClientEmail', "width": "20%" },
            { data: 'primaryClientName', "width": "15%" },
            {
                data: 'creationDate',
                "width": "15%",
                "render": function (data) {
                    var formattedDate = new Date(data).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
                    return formattedDate;
                }
            },
            { data: 'agencyName', "width": "15%" },
            {
                data: 'primaryClientEmail',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <button type="button" onclick="RenderPrimaryClientEditView('${data}');" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</button>
                     <button type="button" onClick="DeleteClient('${data}');" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</button>
                    </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function DeleteClient(email) {
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
                    url: '/primaryclient/delete',
                    type: 'DELETE',
                    data: { email: email },
                    success: function (data) {
                        Swal.fire({
                            title: "Deleted!",
                            text: "Primary Client has been deleted.",
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

function RenderPrimaryClientEditView(email) {
    renderPrimaryClientEditPartialView(email);
}

