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
                "holes": [],
                "id": "00000000-0000-0000-0000-000000000000",
                "imageUri": "",
                "location": "",
                "name": "",
                "playerId": this.playerId
            };
            this.isEdit = true;
            this.isNew = true;
            this.generateHoleCount = 0;
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
                    "id": "00000000-0000-0000-0000-000000000000",
                    "playerId": course.playerId
                };
                this.editedScorecard.scores = [];
                for (let i = 0; i < course.holes.length; i++) {
                    this.editedScorecard.scores[i] = {
                        "par": course.holes[i].par,
                        "score": 0
                    };
                }
                this.isEdit = true;
                this.isNew = true;
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
                this.editedScorecard.date = new Date(this.editedScorecard.date).toLocaleDateString();
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
                this.errors.remove('holes');
                let count = Number(this.generateHoleCount);
                this.editedCourse.holes = [];
                for (let i = 0; i < count; i++) {
                    this.editedCourse.holes.push({
                        "distance": 0,
                        "par": 3
                    });
                }
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
        getScoreClass: function (scorecard, scoreIndex) {
            let score = scorecard.scores[scoreIndex].score;
            let par = scorecard.scores[scoreIndex].par;
            return {
                "badge-light": score === par,
                "badge-pill": score < par,
                "badge-success": score < par,
                "badge-danger": score > par
            };
        },
        getTotalScore: function (scorecard) {
            let par = 0;
            let score = 0;
            for (let i = 0; i < scorecard.scores.length; i++) {
                par += scorecard.scores[i].par;
                score += scorecard.scores[i].score;
            };
            let net = score - par;
            if (net > 0) {
                net = '+' + net;
            }
            return score + ' (' + net + ')';
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
            if (this.isNew) {
                this.$http.post(dgpConfig.api + '/courses', this.editedCourse).then((response) => {
                    this.editedCourse = response.data;
                    let course = new {
                        "holes": this.editedCourse.holes.length,
                        "id": this.editedCourse.id,
                        "name": course.name,
                        "par": 0,
                        "playerId": this.editedCourse.playerId
                    };
                    for (let i = 0; i < this.editedCourse.holes.length; i++) {
                        course.par += this.editedCourse.holes[i].par;
                    }
                    this.courses.push(course);
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
                    let par = 0;
                    for (let i = 0; i < this.editedCourse.holes.length; i++) {
                        par += this.editedCourse.holes[i].par;
                    }
                    course.par = par;
                    this.viewCourses();
                }, response => {
                    this.viewCourses();
                });
            }
        },
        saveScorecard : function() {
            if (this.isNew) {
                this.$http.post(dgpConfig.api + '/scorecards', this.editedScorecard).then((response) => {
                    this.editedScorecard = response.data;
                    let scorecard = new {
                        "courseId": this.editedScorecard.courseId,
                        "date": new Date().toLocaleDateString(),
                        "id": this.editedScorecard.id,
                        "playerId": this.editedScorecard.playerId
                    };
                    for (let i = 0; i < this.editedScorecard.scores.length; i++) {
                        scorecard.scores.push(new {
                            "par": this.editedScorecard.scores[i].par,
                            "score": this.editedScorecard.scores[i].score
                        });
                    }
                    this.scorecards.push(scorecard);
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
                    for (let i = 0; i < scorecard.scores.length; i++){
                        scorecard.scores[i].par = this.editedScorecard.score[i].par;
                        scorecard.scores[i].score = this.editedScorecard.scores[i].score;
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
