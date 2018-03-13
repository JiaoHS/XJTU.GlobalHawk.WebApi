myApp1.controller('HomeIndexController', function ($scope, $http, $location) {
    var map = new BMap.Map("allmap");
    var point = new BMap.Point(108.99201870491, 34.251736393392);
    map.centerAndZoom(point, 19);
    $scope.Init = function () {

        var makerPoint = [];

        //var point = new BMap.Point(108.99201870491, 34.251736393392);
        //map.centerAndZoom(point, 19);
        point = { "lng": point.lng, "lat": point.lat, "carid": 0, "time": 0 };
        makerPoint.push(point);
        map.enableScrollWheelZoom(true);
        $scope.AddMarker(makerPoint, map, 1);

        var myDate = new Date()
        $scope.currentTimestart = $scope.getNowFormatDate();
        $scope.currentTimeend = $scope.getNowFormatDate();
        $('#datetimepickerstart').datetimepicker({
            format: 'yyyy-mm-dd hh:mm:ss',      /*此属性是显示顺序，还有显示顺序是mm-dd-yyyy*/
            autoclose: true,//自动关闭
            minView: 2,//最精准的时间选择为日期0-分 1-时 2-日 3-月 
            weekStart: 0
        });
        $('#datetimepickerend').datetimepicker({
            format: 'yyyy-mm-dd hh:mm:ss',      /*此属性是显示顺序，还有显示顺序是mm-dd-yyyy*/
            autoclose: true,//自动关闭
            minView: 2,//最精准的时间选择为日期0-分 1-时 2-日 3-月 
            weekStart: 0
        });
    }
    $scope.getNowFormatDate = function () {
        var date = new Date();
        var seperator1 = "-";
        var seperator2 = ":";
        var month = date.getMonth() + 1;
        var hour = date.getHours();
        var minu = date.getMinutes();
        var sec = date.getSeconds();
        var strDate = date.getDate();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (hour >= 1 && hour <= 9) {
            hour = "0" + hour;
        }
        if (minu >= 1 && minu <= 9) {
            minu = "0" + minu;
        }
        if (sec >= 1 && sec <= 9) {
            sec = "0" + sec;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + hour + seperator2 + minu
                + seperator2 + sec;
        return currentdate;
    }

    $scope.ApiUrl = getApiUrl();
    $scope.pager = {};
    $scope.FilterPanel = [];
    var pageSize = 15;
    var points = [];//原始点信息数组  
    var bPoints = [];//百度化坐标数组。用于更新显示范围。
    $scope.LoadData = function (pageIndex, pageSize, filterpanel) {
        var pSize = $scope.pager.pageSize = pageSize;
        $http({
            method: "get",
            url: $scope.ApiUrl + "/Home/HomeList?pageIndex=" + pageIndex + "&pageSize=" + pageSize,
            dataType: "json",
            data: JSON.stringify(filterpanel),
            headers: { "Content-Type": "application/json;charset=utf-8" }
        }).then(function (d) {
            $scope.NoticeList = d.data.Data;
            $scope.totalCount = d.data.totalCount;


            //分页
            var totalNum = $scope.pager.totalNumbers = d.data.totalCount;
            $scope.pager.pageCount = Math.ceil(totalNum / pSize);
            $scope.pager.currentPageNumber = pageIndex;
        });
    }

    //分页跳转
    $scope.pageChanged = function (pageIndex) {
        $scope.LoadData(pageIndex, pageSize, $scope.FilterPanel);
    }
    $scope.theLocation = function () {
        if (document.getElementById("longitude").value != "" && document.getElementById("latitude").value != "") {
            map.clearOverlays();
            var new_point = new BMap.Point(document.getElementById("longitude").value, document.getElementById("latitude").value);
            var marker = new BMap.Marker(new_point);  // 创建标注
            map.addOverlay(marker);              // 将标注添加到地图中
            map.panTo(new_point);
        } else {
            map.centerAndZoom(point, 12);
        }
    }
    $scope.LoadTrack = function () {
        //var map = new BMap.Map("allmap");
        //map.enableScrollWheelZoom(true);
        var map = new BMap.Map("allmap");
        map.centerAndZoom(point, 19);
        // $scope.ClearTrack();//清除之前的轨迹
        if ($scope.selectedName == null) {
            alert("车号不能为空！");
            $scope.Init();
            return;
        }
        if ($scope.currentTimestart >= $scope.currentTimeend) {
            alert("开始时间不能大于结束时间！");
            $scope.Init();
            return;
        }
        //$scope.Init();
        var arrs = [];
        var FilterPanel = [];
        $http({
            method: "get",
            url: $scope.ApiUrl + "/Home/HomeList?sn_id=" + $scope.selectedName + "&pageIndex=" + 1 + "&pageSize=" + 15000 + "&startTime=" + $scope.currentTimestart + "&endTime=" + $scope.currentTimeend,
            dataType: "json",
            data: JSON.stringify(FilterPanel),
            headers: { "Content-Type": "application/json;charset=utf-8" }
        }).then(function (d) {
            if (d == null || d.data.Data.length <= 0) {
                alert("无数据！");
                $scope.Init();
                return;
            }
            $scope.carTrackList = d.data.Data;
            angular.forEach($scope.carTrackList, function (value, index) {
                var wgs84togcj02 = coordtransform.wgs84togcj02(value.lng.toFixed(6), value.lat.toFixed(6));
                var gcj02tobd09 = coordtransform.gcj02tobd09(wgs84togcj02[0].toFixed(6), wgs84togcj02[1].toFixed(6));
                //Point = new BMap.Point(gcj02tobd09[0], gcj02tobd09[1]);
                var point = { "lng": gcj02tobd09[0], "lat": gcj02tobd09[1], "carid": value.sn, "time": Number(value.time.replace(/\/Date\((\d+)\)\//, "$1")), "acc": value.accx + "|" + value.accy + "|" + value.accz, "mag": value.magx + "|" + value.magy + "|" + value.magz, "mems": value.memsx + "|" + value.memsy + "|" + value.memsz };
                //$scope.FormateTime(value.time);
                arrs.push(point);


            });
            //dynamicLine(arrs, map);
            dynamicLine2(arrs, map);

        });
    }
    //计算钱前向方向 传参数：数组和当前索引
    function TurnAngleForward(arrs, i, anglesArr) {
        var ang = 0;
        if (i == 0 || i == arrs.length - 1) {
            ang = 0;
            anglesArr.push(ang);
            return;
        }
        var dPointx, dPointy;
        //var lng = arrs[i].lng;
        //var lat = arrs[i].lat;
        //初始化
        var threshold = 0.000000002111;
        var q = 2;
        var error = 0;
        var a = 0, b = 0, last_b;
        var numerator = (arrs[i + 1].lng - arrs[i].lng) * (arrs[i + 1].lat - arrs[i].lat);
        var denominator = (arrs[i + 1].lng - arrs[i].lng) * (arrs[i + 1].lng - arrs[i].lng);
        while (error <= threshold) {
            if (i + q > arrs.length - 1) {
                break;
            }
            last_b = b;
            numerator = numerator + (arrs[i + q].lng - arrs[i].lng) * (arrs[i + q].lat - arrs[i].lat);
            denominator = denominator + (arrs[i + q].lng - arrs[i].lng) * (arrs[i + q].lng - arrs[i].lng);
            if (denominator == 0) {
                b = 975642;
                q = q + 1;
                continue;
            }
            b = numerator / denominator;
            a = arrs[i].lat - b * arrs[i].lng;
            error = 0;
            for (var j = 0; j <= q; j++) {
                error = error + (arrs[i + j].lat - a - b * arrs[i + j].lng) * (arrs[i + j].lat - a - b * arrs[i + j].lng);
            }
            q = q + 1;
        }
        q = q - 1;
        if (last_b != 975642) {//斜率存在
            dPointx = arrs[i + q].lng - arrs[i].lng;
            dPointy = last_b * dPointx;
        } else {
            dPointx = 0;
            dPointy = arrs[i + q].lat - arrs[i].lat;
        }

        //计算后向角度，传参数：数组和当前索引
        var dBackPointx, dBackPointy;
        threshold = 0.00000000211;
        var p = 2;
        error = 0;
        a = 0, b = 0, last_b;
        numerator = (arrs[i - 1].lng - arrs[i].lng) * (arrs[i - 1].lat - arrs[i].lat);
        denominator = (arrs[i - 1].lng - arrs[i].lng) * (arrs[i - 1].lng - arrs[i].lng);
        while (error <= threshold) {
            if (i - p < 0) {
                break;
            }
            last_b = b;
            numerator = numerator + (arrs[i - p].lng - arrs[i].lng) * (arrs[i - p].lat - arrs[i].lat);
            denominator = denominator + (arrs[i - p].lng - arrs[i].lng) * (arrs[i - p].lng - arrs[i].lng);
            if (denominator == 0) {
                b = 975642;
                p = p + 1;
                continue;
            }
            b = numerator / denominator;
            a = arrs[i].lat - b * arrs[i].lng;
            error = 0;
            for (var j = 0; j <= p; j++) {
                error = error + (arrs[i - j].lat - a - b * arrs[i - j].lng) * (arrs[i - j].lat - a - b * arrs[i - j].lng);
            }
            p = p + 1;
        }
        p = p - 1;
        if (last_b != 975642) {//斜率存在
            dBackPointx = arrs[i].lng - arrs[i - p].lng;
            dBackPointy = last_b * dBackPointx;
        } else {
            dBackPointx = 0;
            dBackPointy = arrs[i].lat - arrs[i - p].lat;
        }

        //计算转弯角度
        var temp = dBackPointx * dPointy - dBackPointy * dPointx;
        temp = temp > 0 ? -1 : temp == 0 ? 0 : 1;
        var x = (dBackPointx * dPointx + dPointy * dBackPointy);
        var y = Math.sqrt(dBackPointx * dBackPointx + dBackPointy * dBackPointy) * Math.sqrt(dPointx * dPointx + dPointy * dPointy);
        var tp = x / y;
        if (tp > 1 || tp < -1) {
            tp = 0;
        }
        var angle = temp * Math.acos(tp);//反三角函数求弧度
        ang = Math.floor(180 / (Math.PI / angle));//将弧度转换成角度
        if (isNaN(ang)) {
            ang = 0;
        }
        anglesArr.push(ang);
    }

    //$scope.LoadData(2, pageSize, $scope.FilterPanel);
    //第一版
    function dynamicLine(arrs, map) {
        map.clearOverlays();
        var arrsTemp = [];
        var startIndex = 0;
        var endIndex = 1;
        var myVar = setInterval(function () {
            arrsTemp = arrs.slice(startIndex, endIndex);//取一个
            var len = points.length;
            var lng = arrsTemp[0].lng;
            var lat = arrsTemp[0].lat;
            var point = { "lng": lng, "lat": lat, "carid": arrsTemp[0].carid, "time": arrsTemp[0].time };
            var makerPoints = [];
            var newLinePoints = [];


            makerPoints.push(point);
            if (startIndex == 0 || startIndex == arrs.length - 1) {
                $scope.AddMarker(makerPoints, map, 1);//增加起点 终点的标注  
            } else {
                $scope.AddMarker(makerPoints, map, 2);
            }

            points.push(point);
            bPoints.push(new BMap.Point(lng, lat));
            len = points.length;
            newLinePoints = points.slice(len - 2, len);

            $scope.AddLine(newLinePoints, map);//增加轨迹线  
            $scope.setZoom(bPoints, map);
            startIndex++;
            endIndex = startIndex + 1;
            if (startIndex == arrs.length) {
                clearInterval(myVar);
                alert("OVER");
            }
        }, 100);
    }
    //第二版
    function dynamicLine2(arrs, map) {
        sessionStorage.arrs = JSON.stringify(arrs);
        var anglesArr = [];
        var angArr = [];
        map.enableScrollWheelZoom();
        map.centerAndZoom();
        var marker;
        var lushu;
        var arrPois = arrs;
        map.setViewport(arrPois);
        var makerPoint = [];
        var dic = new Array();
        var behaviors = [];//存储暂停时刻的time和行为id
        if (sessionStorage.behaviors) {
            sessionStorage.behaviors = [];
        }

        var point1 = { "lng": arrPois[0].lng, "lat": arrPois[0].lat, "carid": 0, "time": 0, "title": "起点" };
        var point2 = { "lng": arrPois[arrPois.length - 1].lng, "lat": arrPois[arrPois.length - 1].lat, "carid": 0, "time": 0, "title": "终点" };
        makerPoint.push(point1);
        makerPoint.push(point2);
        $scope.AddMarker(makerPoint, map, 1);
        BMapLib.LuShu.prototype.pause
        BMapLib.LuShu.prototype._move = function (initPos, targetPos, effect) {
            //项目是根据实时坐标数据进行定位，所以存在一个等待新数据的过程，而对于覆盖物的坐标改变就是一个setPosition(BMap.Point)方法而已也就造成了停顿。所以目前暂且解决方案就是：让他这个覆盖物在这个等待的期间找点事情做，不要一下就直接从起点蹦到终点了，慢慢的移动过去。小碎步，平滑的的移动过去
            //此时这个事情就可以转化为已知起始点坐标，进行移动覆盖物的这么过程了，说白了就是让他覆盖物在两个点连成的这条线上多执行几次setPosition(BMap.Point)，一次步子别迈那么大，只要保证在下次新坐标来之前到达就行了。
            var pointsArr = [initPos, targetPos];  //点数组
            var me = this,
                //当前的帧数
                currentCount = 0,
                //步长，米/秒
                timer = 18,
                step = this._opts.speed / (1000 / timer),
                //初始坐标
                init_pos = this._projection.lngLatToPoint(initPos),//获取到的经纬度坐标是球面坐标，所以要先转换为平面坐标
                //获取结束点的(x,y)坐标
                target_pos = this._projection.lngLatToPoint(targetPos),
                //总的步长
                count = Math.round(me._getDistance(init_pos, target_pos) / step);
            //显示折线 
            //this._map.addOverlay(new BMap.Polyline(pointsArr, {
            //    strokeColor: "blue",
            //    strokeWeight: 5,
            //    strokeOpacity: 1
            //})); // 画线 
            $scope.AddLine(pointsArr, map);
            //如果小于1直接移动到下一点
            if (count < 1) {
                me._moveNext(++me.i);
                return;
            }
            me._intervalFlag = setInterval(function () {
                //两点之间当前帧数大于总帧数的时候，则说明已经完成移动
                if (currentCount >= count) {
                    clearInterval(me._intervalFlag);
                    //移动的点已经超过总的长度
                    if (me.i > me._path.length) {
                        return;
                    }
                    //运行下一个点
                    me._moveNext(++me.i);
                } else {
                    currentCount++;
                    var x = effect(init_pos.x, target_pos.x, currentCount, count),
                        y = effect(init_pos.y, target_pos.y, currentCount, count),
                        pos = me._projection.pointToLngLat(new BMap.Pixel(x, y));
                    //设置marker
                    if (currentCount == 1) {
                        var proPos = null;
                        if (me.i - 1 >= 0) {
                            proPos = me._path[me.i - 1];
                        }
                        if (me._opts.enableRotation == true) {
                            SetMarkerRotation(me._marker, initPos, targetPos, angArr);
                            //me.setRotation(proPos, initPos, targetPos);//改变覆盖物的旋转角度
                        }
                        if (me._opts.autoView) {
                            if (!me._map.getBounds().containsPoint(pos)) {
                                me._map.setCenter(pos);
                            }
                        }
                    }
                    //正在移动
                    me._marker.setPosition(pos);
                    //设置自定义overlay的位置
                    me._setInfoWin(pos);
                }
            }, timer);
        };
        var landmarkPois = [];
        for (var i = 0; i < arrPois.length; i++) {
            TurnAngleForward(arrPois, i, anglesArr);
            var sContent =
      '<ul style="margin:0 0 5px 0;padding:0.2em 0">'
      + '<li style="line-height: 26px;font-size: 15px;">'
      + '<span style="width: 50px;display: inline-block;">CarId：</span>' + arrPois[i].carid + '</li>'
      + '<li style="line-height: 26px;font-size: 15px;">'
      + '<span style="width: 50px;display: inline-block;">lng：</span>' + arrPois[i].lng + '</li>'
         + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">lat：</span>' + arrPois[i].lat + '</li>'
          + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">acc：</span>' + arrPois[i].acc + '</li>'
          + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">mag：</span>' + arrPois[i].mag + '</li>'
          + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">mems：</span>' + arrPois[i].mems + '</li>'
          + '<li style="line-height: 26px;font-size: 15px;">'
            + '<span style="width: 50px;display: inline-block;">angle：</span>' + anglesArr[i] + '</li>'
          + '<li style="line-height: 26px;font-size: 15px;">'
           //+ '<li style="line-height: 26px;font-size: 15px;">'
           //+ '<span style="width: 50px;display: inline-block;">Time：</span>' + point.time + '</li>'
      + '</ul>';
            landmarkPois.push({ lng: arrPois[i].lng, lat: arrPois[i].lat, html: sContent, pauseTime: 0 });
        }
        lushu = new BMapLib.LuShu(map, arrPois, {
            defaultContent: "",
            autoView: true,//是否开启自动视野调整，如果开启那么路书在运动过程中会根据视野自动调整
            icon: new BMap.Icon('../images/car5.png', new BMap.Size(52, 26), { anchor: new BMap.Size(26, 13) }),
            speed: 150,//覆盖物移动速度，单位米/秒 
            enableRotation: true,//是否设置marker随着道路的走向进行旋转
            landmarkPois: landmarkPois
        });
        //marker.addEventListener("click", function () {
        //    alert("hhh");
        //});
        $scope.Start = function () {
            //marker.enableMassClear(); //设置后可以隐藏改点的覆盖物
            //marker.hide();
            lushu.start();
            //lushu.showInfoWindow();
            //map.clearOverlays();    //清除所有覆盖物
        }
        $scope.Show = function () {
            lushu.showInfoWindow();
        }
        $scope.Pause = function () {
            if ($scope.behaviorId == null || $scope.behaviorId == "") {
                lushu.pause();
                alert("请输入驾驶行为id");
                return;
            }
            if (sessionStorage.behaviorId == $scope.behaviorId) {
                lushu.pause();
                alert("与上次选择一样是否更改！");
                sessionStorage.behaviorId = "";
                return;
            }
            var index = lushu.i;//取到当前数组小车的索引
            var time = lushu._path[index].time;//取到当前小车运行的时间
            var tempJson = {};
            tempJson.time = time;
            tempJson.behaviorId = $scope.behaviorId;
            behaviors.push(tempJson);
            sessionStorage.behaviors = JSON.stringify(behaviors);
            sessionStorage.behaviorId = $scope.behaviorId;
        }
        $scope.analyze = function () {
            //sessionStorage.behaviors = '[{"time":1498312800000,"behaviorId":"5"},{"time":1498312800000,"behaviorId":"5"},{"time":1498312800000,"behaviorId":"1"},{"time":1498312800000,"behaviorId":"1"},{"time":1498312980000,"behaviorId":"1"},{"time":1498312980000,"behaviorId":"1"},{"time":1498313160000,"behaviorId":"1"},{"time":1498313220000,"behaviorId":"1"},{"time":1498313220000,"behaviorId":"1"},{"time":1498313340000,"behaviorId":"1"},{"time":1498313400000,"behaviorId":"1"},{"time":1498313460000,"behaviorId":"1"},{"time":1498313460000,"behaviorId":"1"},{"time":1498313460000,"behaviorId":"1"},{"time":1498313700000,"behaviorId":"1"},{"time":1498313820000,"behaviorId":"1"},{"time":1498313820000,"behaviorId":"1"},{"time":1498313820000,"behaviorId":"1"},{"time":1498313820000,"behaviorId":"1"},{"time":1498313880000,"behaviorId":"1"},{"time":1498314000000,"behaviorId":"1"},{"time":1498314060000,"behaviorId":"1"},{"time":1498314240000,"behaviorId":"1"}]';
            $http({
                method: "post",
                url: $scope.ApiUrl + "/Home/CarSample",
                dataType: "json",
                data: JSON.stringify({ 'sample': sessionStorage.behaviors, "snId": $scope.selectedName }),
                headers: { "Content-Type": "application/json;charset=utf-8" }
            }).then(function (d) {
                if (d.data.Data == "ok") {
                    alert("完成！");
                } else {
                    alert("失败！");
                }
            });
            //开始比较数据
            //var arrs = sessionStorage.arrs;


            //angular.forEach(sessionStorage.behaviors, function (value, index) {
            //    var Samples = [];//样本采集数组
            //    var sample = {};
            //    var tempArr = [];
            //    var tempJson = {};
            //    for (var i = 0; i < arrs.length; i++) {
            //        if (arrs[i].time == value.time) {
            //            tempJson.accx = arrs[i].accx;
            //            tempJson.accy = arrs[i].accy;
            //            tempJson.accz = arrs[i].accz;
            //            tempJson.lng = arrs[i].lng;
            //            tempJson.lat = arrs[i].lat;
            //            tempJson.memsx = arrs[i].memsx;
            //            tempJson.memsy = arrs[i].memsy;
            //            tempJson.memsz = arrs[i].memsz
            //            tempArr.push(tempJson);
            //        }
            //    }
            //    sample.arrs = tempArr;
            //    sample.lable = value.behaviorId;
            //    Samples.push(sample);
            //});
        }
    }

    //设置车辆的角度
    function SetMarkerRotation(marker, curPos, targetPos, angArr) {

        var deg = 0;
        //start!
        curPos = map.pointToPixel(curPos);
        targetPos = map.pointToPixel(targetPos);

        if (targetPos.x != curPos.x) {
            var tan = (targetPos.y - curPos.y) / (targetPos.x - curPos.x),
                atan = Math.atan(tan);
            deg = atan * 360 / (2 * Math.PI);
            //degree  correction;
            if (targetPos.x < curPos.x) {
                deg = -deg + 90 + 90;

            } else {
                deg = -deg;
            }

            marker.setRotation(-deg);
            angArr.push(-deg);
        } else {
            var disy = targetPos.y - curPos.y;
            var bias = 0;
            if (disy > 0)
                bias = -1
            else
                bias = 1
            marker.setRotation(-bias * 90);
            angArr.push(-bias * 90);
        }

        return;

    }


    $scope.AddMarker = function (points, map, type) {
        //循环建立标注点
        for (var i = 0, pointsLen = points.length; i < pointsLen; i++) {
            var point = new BMap.Point(points[i].lng, points[i].lat); //将标注点转化成地图上的点
            var marker = new BMap.Marker(point); //将点转化成标注点
            marker.setAnimation(BMAP_ANIMATION_BOUNCE);
            var label = new BMap.Label("points" + points[i].lng + "-" + points[i].lat, { offset: new BMap.Size(20, -10) });
            if (points[i].title != "" && points[i].title != "undefined" && points[i].title != undefined) {
                label = new BMap.Label(points[i].title, { offset: new BMap.Size(20, -10) });
                label.setStyle({ border: "1px solid rgb(204, 204, 204)", color: "rgb(0, 0, 0)", borderRadius: "10px", padding: "5px", background: "rgb(255, 255, 255)", });
            }
            marker.setLabel(label);
            if (type == 1) {
                map.addOverlay(marker);  //将标注点添加到地图上
                //添加监听事件
                (function () {
                    var thePoint = points[i];
                    marker.addEventListener("click",
                        function () {
                            $scope.showInfo(this, thePoint);
                        });
                })();
            }
            if (type == 2) {
                //map.addOverlay(marker);  //将标注点添加到地图上
                //添加监听事件
                (function () {
                    var thePoint = points[i];
                    marker.addEventListener("click",
                        function () {
                            $scope.showInfo(this, thePoint);
                        });
                })();
            }

        }
    }

    $scope.AddLine = function (points, map) {
        var lushu;
        var linePoints = [], pointsLen = points.length, i, polyline;
        if (pointsLen == 0) {
            return;
        }
        // 创建标注对象并添加到地图     
        for (i = 0; i < pointsLen; i++) {
            linePoints.push(new BMap.Point(points[i].lng, points[i].lat));
        }

        polyline = new BMap.Polyline(linePoints, { strokeColor: "blue", strokeWeight: 5, strokeOpacity: 1 });   //创建折线  
        map.addOverlay(polyline);   //增加折线  


    }
    $scope.setZoom = function (bPoints, map) {
        var view = map.getViewport(eval(bPoints));
        var mapZoom = view.zoom;
        var centerPoint = view.center;
        map.centerAndZoom(centerPoint, mapZoom);
    }
    $scope.showInfo = function (thisMarker, point) {
        var sContent =
        '<ul style="margin:0 0 5px 0;padding:0.2em 0">'
        + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">CarId：</span>' + point.carid + '</li>'
        + '<li style="line-height: 26px;font-size: 15px;">'
        + '<span style="width: 50px;display: inline-block;">lng：</span>' + point.lng + '</li>'
           + '<li style="line-height: 26px;font-size: 15px;">'
          + '<span style="width: 50px;display: inline-block;">lat：</span>' + point.lat + '</li>'
             //+ '<li style="line-height: 26px;font-size: 15px;">'
             //+ '<span style="width: 50px;display: inline-block;">Time：</span>' + point.time + '</li>'
        + '</ul>';
        var infoWindow = new BMap.InfoWindow(sContent); //创建信息窗口对象
        thisMarker.openInfoWindow(infoWindow); //图片加载完后重绘infoWindow
    }
    $scope.FormateTime = function (str) {
        str = str.replace("/Date(", "").replace(")/", "");
        var now = new Date(parseInt(str) * 1000).toLocaleString().replace(/:\d{1,2}$/, ' ')
        //var year = now.getYear();
        //var month = now.getMonth() + 1;
        //var date = now.getDate();
        //var hour = now.getHours();
        //var minute = now.getMinutes();
        //var second = now.getSeconds();

        alert(now);
    }
    $scope.ClearTrack = function () {
        map.clearOverlays();//清除图层覆盖物
    }
    $scope.LoadselectList = function (pageIndex, pageSize, filterpanel) {
        var pSize = $scope.pager.pageSize = pageSize;
        $http({
            method: "get",
            url: $scope.ApiUrl + "/Home/CarNumList?pageIndex=" + pageIndex + "&pageSize=" + pageSize,
            dataType: "json",
            data: JSON.stringify(filterpanel),
            headers: { "Content-Type": "application/json;charset=utf-8" }
        }).then(function (d) {
            $scope.config2 = d.data.Data;
            $scope.totalCount = d.data.totalCount;
            $scope.selectList = [];
            $scope.selectedName = 0;
            angular.forEach($scope.config2, function (value, index) {
                var temp = { "id": index + 1, "snNum": value.sn };
                //$scope.FormateTime(value.time);
                $scope.selectList.push(temp);
            });
            if ($scope.selectList.length > 0) {
                $scope.selectedName = $scope.selectList[0].snNum;
            }

        });
    }

    //加载车辆列表
    $scope.LoadselectList(1, 15, $scope.FilterPanel);
    $scope.BehaviorIds = [{ id: 1, name: '小于等于90拐弯' },{ id: 2, name: '大于90拐弯' },{ id: 3, name: '直线' }, { id: 3, name: 'S型' }, { id: 4, name: 'U型' }];
    $scope.Init();


});
myApp1.directive('select2', function (select2Query) {
    return {
        restrict: 'A',
        scope: {
            config: '=',
            ngModel: '=',
            select2Model: '='
        },
        link: function (scope, element, attrs) {
            // 初始化
            var tagName = element[0].tagName,
                config = {
                    allowClear: true,
                    multiple: !!attrs.multiple,
                    placeholder: attrs.placeholder || ' '   // 修复不出现删除按钮的情况
                };

            // 生成select
            if (tagName === 'SELECT') {
                // 初始化
                var $element = $(element);
                delete config.multiple;

                $element
                    .prepend('<option value=""></option>')
                    .val('')
                    .select2(config);

                // model - view
                scope.$watch('ngModel', function (newVal) {
                    setTimeout(function () {
                        $element.find('[value^="?"]').remove();    // 清除错误的数据
                        $element.select2('val', newVal);
                    }, 0);
                }, true);
                return false;
            }

            // 处理input
            if (tagName === 'INPUT') {
                // 初始化
                var $element = $(element);

                // 获取内置配置
                if (attrs.query) {
                    scope.config = select2Query[attrs.query]();
                }

                // 动态生成select2
                scope.$watch('config', function () {
                    angular.extend(config, scope.config);
                    $element.select2('destroy').select2(config);
                }, true);

                // view - model
                $element.on('change', function () {
                    scope.$apply(function () {
                        scope.select2Model = $element.select2('data');
                    });
                });

                // model - view
                scope.$watch('select2Model', function (newVal) {
                    $element.select2('data', newVal);
                }, true);

                // model - view
                scope.$watch('ngModel', function (newVal) {
                    // 跳过ajax方式以及多选情况
                    if (config.ajax || config.multiple) { return false }

                    $element.select2('val', newVal);
                }, true);
            }
        }
    }
});

/**
 * select2 内置查询功能
 */
myApp1.factory('select2Query', function ($timeout) {
    return {
        testAJAX: function () {
            var config = {
                minimumInputLength: 1,
                ajax: {
                    url: "http://api.rottentomatoes.com/api/public/v1.0/movies.json",
                    dataType: 'jsonp',
                    data: function (term) {
                        return {
                            q: term,
                            page_limit: 10,
                            apikey: "ju6z9mjyajq2djue3gbvv26t"
                        };
                    },
                    results: function (data, page) {
                        return { results: data.movies };
                    }
                },
                formatResult: function (data) {
                    return data.title;
                },
                formatSelection: function (data) {
                    return data.title;
                }
            };

            return config;
        }
    }
});
//angular.bootstrap(document, ['myApp1']);