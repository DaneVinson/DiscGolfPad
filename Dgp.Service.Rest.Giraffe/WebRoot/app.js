function sum(total, current) {
    return total + current;
}

Vue.use(VeeValidate);
var vm = new Vue({
    el: '#app',
    data: {
        caption: 'Courses',
        editedCourse: null,
        editedScorecard: null,
        courses: [],
        currentCourse: null,
        generateHoleCount: 0,
        isBusy: false,
        isEdit: false,
        isDelete: false,
        isNew: false,
        mode: 'Courses', /* Courses, Scorecards, Course, Scorecard */
        playerId: '',
        scorecards: []
    },
    created: function() {
        this.isBusy = true;
        this.playerId = dgpConfig.playerId;
        this.$http.get(dgpConfig.api + '/courses').then((response) => {
            this.courses = response.data;
            this.isBusy = false;
        }, response => {
            this.courses = [];
            this.isBusy = false;
        });
    },
    methods: {
        add: function () {
            if (this.mode === 'Courses') {
                return this.addCourse();
            }
            else if (this.mode === 'Scorecards') {
                return this.addScorecard(this.currentCourse.id);
            }
        },
        addCourse: function () {
            this.editedCourse = {
                "holeDistances": [],
                "holePars": [],
                "id": "",
                "imageUri": "",
                "location": "",
                "name": "",
                "playerId": this.playerId
            };
            this.isEdit = true;
            this.isNew = true;
            this.generateHoleCount = 0;
            this.errors.clear();
            this.errors.add({ 'field': 'location', 'msg': 'Requried' });
            this.errors.add({ 'field': 'name', 'msg': 'Requried' });
            this.errors.add({ 'field': 'holeCount', 'msg': 'At least one hole is required.' });
            this.errors.add({ 'field': 'holes', 'msg': 'At least one hole is required.' });
            this.caption = 'New Course';
            this.mode = 'Course';
        },
        addScorecard : function(courseId) {
            this.isBusy = true;
            this.$http.get(dgpConfig.api + '/courses/' + courseId).then((response) => {
                let course = response.data;
                this.editedScorecard = {
                    "courseId": course.id,
                    "date": new Date().toLocaleDateString(),
                    "holePars": course.holePars,
                    "holeScores": new Array(course.holePars.length).fill(0),
                    "id": "",
                    "playerId": course.playerId
                };
                this.isEdit = true;
                this.isNew = true;
                this.errors.clear();
                this.errors.add({ 'field': 'date', 'msg': 'Requried' });
                for (let i = 0; i < course.holePars.length; i++) {
                    this.errors.add({ 'field': 'scores' + i, 'msg': 'Must be greater than 0.' });
                }
                this.caption = 'New ' + course.name + ' Scorecard';
                this.isBusy = false;
                this.currentCourse = course;
                this.mode = 'Scorecard';
            }, response => {
                this.viewScorecards();
            });
        },
        cancel: function() {
            this.errors.clear();
            this.isDelete = false;
            this.isEdit = false;
            this.isNew = false;
            this.mode === ('Scorecard' && this.currentCourse) ? this.viewScorecards() : this.viewCourses();
        },
        deleteCourse : function() {
            this.isBusy = true;
            this.$http.delete(dgpConfig.api + '/courses/' + this.editedCourse.id).then((response) => {
                this.courses.splice(this.courses.indexOf(c => c.id === this.editedCourse.id), 1);
                this.viewCourses();
            }, response => {
                this.viewCourses();
            });
        },
        deleteScorecard : function() {
            this.isBusy = true;
            this.$http.delete(dgpConfig.api + '/scorecards/' + this.editedScorecard.id).then((response) => {
                this.scorecards.splice(this.scorecards.indexOf(s => s.id === this.editedScorecard.id), 1);
                this.viewScorecards();
            }, response => {
                this.viewScorecards();
            });
        },
        editCourse: function(courseId) {
            this.isBusy = true;
            this.$http.get(dgpConfig.api + '/courses/' + courseId).then((response) => {
                this.currentCourse = response.data;
                this.editedCourse = response.data;
                this.isEdit = true;
                this.caption = 'Edit Course';
                this.mode = 'Course';
                this.isBusy = false;
            }, response => {
                this.viewCourses();
            });
        },
        editScorecard : function(scorecardId) {
            this.isBusy = true;
            this.$http.get(dgpConfig.api + '/scorecards/' + scorecardId).then((response) => {
                this.currentScorecard = response.data;
                this.editedScorecard = response.data;
                this.isEdit = true;
                this.caption = 'Edit ' + this.currentCourse.name + ' Scorecard';
                this.mode = 'Scorecard';
                this.isBusy = false;
            }, response => {
                this.viewScorecards();
            });
        },
        generateHoles : function() {
            if (this.editedCourse && this.generateHoleCount) {
                let count = Number(this.generateHoleCount);
                this.editedCourse.holeDistances = new Array(count).fill(0);
                this.editedCourse.holePars = new Array(count).fill(3);
                this.errors.remove('holes');
            }
        },
        getSaveButtonClass : function() {
            return {
                'btn-success': !this.isDelete,
                'btn-danger': this.isDelete
            };
        },
        getScorecards : function() {
            this.isBusy = true;
            this.$http.get(dgpConfig.api + '/scorecards').then((response) => {
                this.isBusy = false;
                return response.data;
            }, response => {
                this.isBusy = false;
                return [];
            });
        },
        getScoreClass : function(scorecard, scoreIndex) {
            let score = scorecard.holeScores[scoreIndex];
            let par = scorecard.holePars[scoreIndex];
            return {
                "badge-light": score === par,
                "badge-pill": score < par,
                "badge-success": score < par,
                "badge-danger": score > par
            };
        },
        getTotalScore : function(scorecard) {
            let par = scorecard.holePars.reduce(sum);
            let score = scorecard.holeScores.reduce(sum);
            let net = score - par;
            if (net > 0) {
                net = '+' + net;
            }
            return score + ' (' + net + ')';
        },
        holesDisplay : function(scorecardId) {
            let scorecard = this.scorecards.find(s => s.id === scorecardId);
            let scores = scorecard.holes[0].score;
            for (let i = 1; i < scorecard.holes.length; i++)
            {
                scores += ' - ' + scorecard.holes[i].score;
            }
            return scores;
        },
        markForDelete: function() {
            if (this.mode === 'Course' || this.mode === 'Scorecard') {
                this.isDelete = true;
            }
        },
        save : function() {
            if (this.mode === 'Course') {
                if (this.isDelete) {
                    this.deleteCourse();
                }
                else {
                    this.saveCourse();
                }
            }
            else if (this.mode === 'Scorecard') {
                if (this.isDelete) {
                    this.deleteScorecard();
                }
                else {
                    this.saveScorecard();
                }
            }
        },
        saveCourse : function() {
            this.isBusy = true;
            if (this.isNew){
                this.$http.post(dgpConfig.api + '/courses', this.editedCourse).then((response) => {
                    this.editedCourse = response.data;
                    this.courses.push(new {
                        "holes": this.editedCourse.holePars.length,
                        "id": this.editedCourse.id,
                        "name": course.name, 
                        "par": this.editedCourse.holePars.reduce(sum), 
                        "playerId": this.editedCourse.playerId
                    });
                    this.viewCourses();
                }, response => {
                    this.viewCourses();
                });
            }
            else {
                this.$http.put(dgpConfig.api + '/courses/' + this.editedCourse.id, this.editedCourse).then((response) => {
                    this.editedCourse = response.data;
                    let course = this.courses.find(c => c.id === this.editedCourse.id);
                    course.imageUri = this.editedCourse.imageUri;
                    course.location = this.editedCourse.location;
                    course.name = this.editedCourse.name;
                    course.par = this.editedCourse.holePars.reduce(sum);
                    this.viewCourses();
                }, response => {
                    this.viewCourses();
                });
            }
        },
        saveScorecard : function() {
            if (this.isNew){
                this.$http.post(dgpConfig.api + '/scorecards', this.editedScorecard).then((response) => {
                    this.editedScorecard = response.data;
                    this.scorecards.push(new {
                        "courseId": this.editedScorecard.courseId,
                        "date": new Date().toLocaleDateString(),
                        "holePars": this.editedScorecard.holePars,
                        "holeScores": new Array(this.editedScorecard.holePars.length).fill(0),
                        "id": this.editedScorecard.id,
                        "playerId": this.editedScorecard.playerId    
                    });
                    this.viewCourses();
                }, response => {
                    this.viewCourses();
                });
            }
            else {
                this.$http.put(dgpConfig.api + '/scorecards/' + this.editedScorecard.id, this.editedScorecard).then((response) => { 
                    this.editedScorecard = response.data;
                    let scorecard = this.scorecards.find(s => s.id === this.editedScorecard.id);
                    scorecard.date = this.editedScorecard.date;
                    for (let i = 0; i < scorecard.holePars.length; i++){
                        scorecard.holeDistances[i] = this.editedScorecard.holeDistances[i];
                        scorecard.holePars[i] = this.editedScorecard.holeDistances[i];
                    }
                    this.viewScorecards();
                }, response => {
                    this.viewScorecards();
                });
            }
        },
        viewCourses : function() {
            this.currentCourse = null;
            this.caption = 'Courses';
            this.editedCourse = null;
            this.editedScorecard = null;
            this.isDelete = false;
            this.isEdit = false;
            this.isNew = false;
            this.isBusy = false;
            this.mode = 'Courses';
        },
        viewScorecards : function(courseId) {
            if (courseId) {
                this.currentCourse = this.courses.find(c => c.id === courseId);
            }
            this.caption = this.currentCourse.name + ' Scorecards';
            this.editedScorecard = null;
            this.isDelete = false;
            this.isEdit = false;
            this.isNew = false;
            this.isBusy = true;
            this.$http.get(dgpConfig.api + '/scorecards?courseId=' + this.currentCourse.id).then((response) => {
                this.scorecards = response.data;
                this.isBusy = false;
                this.mode = 'Scorecards';
            }, response => {
                this.scorecards = [];
                this.isBusy = false;
                this.mode = 'Scorecards';
            });
        }
    }
});
