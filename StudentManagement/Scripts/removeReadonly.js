function removeReadonly() {
    document.getElementById('firstName').removeAttribute('readonly');
    document.getElementById('lastName').removeAttribute('readonly');
    document.getElementById('studentStatus').disabled = false;
    //document.getElementById('programAdvisor').removeAttribute('readonly');
    //document.getElementById('programAdvisor').disabled = false;

    let saveButton = document.getElementById('saveBtn').style.display = 'inline';
    document.getElementById('mode').textContent = 'Edit Mode';
}