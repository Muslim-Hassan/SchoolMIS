$("#UserForm").validate({
    rules: {
        UserName: {
            required: true,
            minlength: 3,
        },
        Password: {
            required: true,
            minlength: 4
        },
    },
    message: {
        
        UserName: {
           
            required:"نوم خانه ډکول مهمه ده",
            minlength: "نوم باید د دری حروفو کم نه وی",
        },
        Password: {
           
            required: "کوډ خانه ډکول مهمه ده",
            minlength: "کوډ باید د څلورو حروفو کم نه وی",
        }
    }
});

////this is for user Type table 
$("#userTypeForm").validate({
    rules: {
        Type: {
            required: true,
            minlength: 4,
        },
        Description: {
            required: true,
            minlength: 10,
        },
    },
    message: {

        Type: {

            required: "نوم خانه ډکول مهمه ده",
            minlength: "نوم باید د دری حروفو کم نه وی",
        },
        Description: {

            required: "کوډ خانه ډکول مهمه ده",
            minlength: "کوډ باید د څلورو حروفو کم نه وی",
        }
    }
});

/////////////////////////////////////////this is for staff section


