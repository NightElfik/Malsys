/// <reference path="~/Js/jquery.js" />

(function ($) {

	function getMousePos(canvas, evt) {
		// get canvas position
		var obj = canvas;
		var top = 0;
		var left = 0;
		while (obj.tagName != 'BODY') {
			top += obj.offsetTop;
			left += obj.offsetLeft;
			obj = obj.offsetParent;
		}

		// return relative mouse position
		var mouseX = evt.clientX - left + window.pageXOffset;
		var mouseY = evt.clientY - top + window.pageYOffset;
		return {
			x: mouseX,
			y: mouseY
		};
	}

	var margin = 10;

	var EDGETYPE = {
		UNKNOWN: 'unknown',
		LINE: 'line',
		LEFT: 'left',
		RIGHT: 'right'
	};

	var CELLPOS = {
		CENTER: 'center',
		TOP: 'top',
		RIGHT: 'right',
		BOTTOM: 'bottom',
		LEFT: 'left'
	};

	var EDGEDIR = {
		UNKNOWN: 'unknown',
		UP: 'up',
		RIGHT: 'right',
		DOWN: 'down',
		LEFT: 'left'
	};

	function edgeTypeToSymbol(edgeType) {
		switch (edgeType) {
			case EDGETYPE.LINE: return 'F';
			case EDGETYPE.LEFT: return 'L';
			case EDGETYPE.RIGHT: return 'R';
			default: return '?';
		}
	}

	function inversEdge(edgeDir) {
		switch (edgeDir) {
			case EDGEDIR.UP: return EDGEDIR.DOWN;
			case EDGEDIR.RIGHT: return EDGEDIR.LEFT;
			case EDGEDIR.DOWN: return EDGEDIR.UP;
			case EDGEDIR.LEFT: return EDGEDIR.RIGHT;
			default: return edgeDir;
		}
	}

	function inverseCellPos(cellPos) {
		switch (cellPos) {
			case CELLPOS.TOP: return CELLPOS.BOTTOM;
			case CELLPOS.RIGHT: return CELLPOS.LEFT;
			case CELLPOS.BOTTOM: return CELLPOS.TOP;
			case CELLPOS.LEFT: return CELLPOS.RIGHT;
			default: return cellPos;
		}
	}

	function getInnerCellPosition(normCellX, normCellY) {
		// normalized coordinates inside one cell (from 0 to 1)

		// center square with edge 30% of square edge
		if (Math.max(Math.abs(normCellX - 0.5), Math.abs(normCellY - 0.5)) < 0.15) {
			return CELLPOS.CENTER;
		}

		var negX = 1 - normCellX;
		if (normCellX < normCellY) {
			if (negX < normCellY) {
				return CELLPOS.BOTTOM;
			}
			else {
				return CELLPOS.LEFT;
			}
		}
		else {
			if (negX < normCellY) {
				return CELLPOS.RIGHT;
			}
			else {
				return CELLPOS.TOP;
			}
		}
	}

	function drawCellLine(g, cellPos, cellX, cellY, cellWid, cellHei) {

		if (cellPos == CELLPOS.CENTER) {
			g.beginPath();
			g.strokeStyle = '#A00';
			g.lineWidth = 8;
			g.lineCap = 'round';
			g.moveTo(cellX, cellY);
			g.lineTo(cellX + cellWid, cellY + cellHei);
			g.moveTo(cellX + cellWid, cellY);
			g.lineTo(cellX, cellY + cellHei);
			g.stroke();
		}

		g.translate(cellX + cellWid / 2, cellY + cellHei / 2);
		var wid, hei;

		switch (cellPos) {
			case CELLPOS.TOP: wid = cellWid; hei = cellHei; break;
			case CELLPOS.RIGHT: g.rotate(Math.PI / 2); wid = cellHei; hei = cellWid; break;
			case CELLPOS.BOTTOM: g.rotate(Math.PI); wid = cellWid; hei = cellHei; break;
			case CELLPOS.LEFT: g.rotate(-Math.PI / 2); wid = cellHei; hei = cellWid; break;
			default: g.setTransform(1, 0, 0, 1, 0, 0); return;
		}
		g.beginPath();
		g.moveTo(-wid / 3, -hei / 2);
		g.lineTo(wid / 3, -hei / 2);
		g.lineTo(wid / 3, hei / 3);
		g.lineTo(-wid / 3, hei / 3);
		g.closePath();
		g.fillStyle = '#DDD';
		g.fill();
		g.lineWidth = 2;
		g.strokeStyle = 'black';
		g.stroke();

		g.beginPath();
		g.moveTo(-wid / 2, -hei / 2);
		g.lineTo(wid / 2, -hei / 2);
		g.lineWidth = 8;
		g.lineCap = 'round';
		g.stroke();

		g.setTransform(1, 0, 0, 1, 0, 0);  // reset transform
	}

	function drawGrid(g, cols, rows, cellWid, cellHei) {
		g.beginPath();
		g.lineWidth = 2;
		g.strokeStyle = '#DDD';
		var wid = cols * cellWid + margin;
		var hei = rows * cellHei + margin;

		for (var y = 0; y <= rows; y++) {
			var actualY = y * cellHei + margin;
			g.moveTo(margin, actualY);
			g.lineTo(wid, actualY);
		}
		for (var x = 0; x <= cols; x++) {
			var actualX = x * cellWid + margin;
			g.moveTo(actualX, margin);
			g.lineTo(actualX, hei);
		}
		g.stroke();
	}

	function drawArrow(g, fromX, fromY, toX, toY, headLen) {
		var angle = Math.atan2(toY - fromY, toX - fromX);
		g.moveTo(fromX, fromY);
		g.lineTo(toX, toY);
		g.moveTo(toX, toY);
		g.lineTo(toX - headLen * Math.cos(angle - Math.PI / 6), toY - headLen * Math.sin(angle - Math.PI / 6));
		g.moveTo(toX, toY);
		g.lineTo(toX - headLen * Math.cos(angle + Math.PI / 6), toY - headLen * Math.sin(angle + Math.PI / 6));
	}

	function drawStartAdnEnd(g, type, wid, hei) {

		var text = type.toUpperCase();
		g.font = '60px sans-serif';
		g.fillStyle = '#EEE';
		g.fillText(text, (wid - g.measureText(text).width) / 2 + margin, hei / 2);

		var y = margin + (type == EDGETYPE.LEFT ? hei : 0);
		var ySign = type == EDGETYPE.LEFT ? -1 : 1

		g.beginPath();
		g.arc(margin, y, 8, 0, 2 * Math.PI, false);
		g.fillStyle = '#0A0';
		g.fill();
		g.lineWidth = 2;
		g.strokeStyle = '#070';
		g.stroke();

		g.beginPath();
		g.arc(margin + wid, y, 8, 0, 2 * Math.PI, false);
		g.fill();
		g.stroke();

		g.beginPath();
		drawArrow(g, margin * 3, y, wid - margin, y, 8);
		g.stroke();

		var endText = 'end';
		var textY = y + 3 * margin * ySign + (type == EDGETYPE.LEFT ? 0 : 10);
		g.font = '16px sans-serif';
		g.fillText('start', margin * 3, textY);
		g.fillText(endText, wid - margin - g.measureText(endText).width, textY);

	}

	function redrawCells(g, data, cellWid, cellHei) {
		var rows = data.length;
		var cols = data[0].length;

		for (var y = 0; y < rows; y++) {
			for (var x = 0; x < cols; x++) {
				if (data[y][x] != CELLPOS.CENTER) {
					drawCellLine(g, data[y][x], x * cellWid + margin, y * cellHei + margin, cellWid, cellHei);
				}
			}
		}
	}

	// returns graph represented as two-dimensional grid of nodes, each node have list of edge directions
	// each entry of neighbors list is array which contains: edge direction, edge type (of original node), x and y coord (of original node)
	function convertToGraph(data) {
		var rows = data.length;
		var cols = data[0].length;

		var nodesRowCount = rows + 1;
		var nodesRowLen = cols + 1;

		var neighbors = new Array(nodesRowCount);

		for (var y = 0; y < nodesRowCount; y++) {
			neighbors[y] = new Array(nodesRowLen);
			for (var x = 0; x < nodesRowLen; x++) {
				neighbors[y][x] = [];
			}
		}

		for (var y = 0; y < rows; y++) {
			for (var x = 0; x < cols; x++) {
				var fromNode, toNode;
				switch (data[y][x]) {
					case CELLPOS.TOP:
						neighbors[y][x].push([EDGEDIR.RIGHT, EDGETYPE.RIGHT, x, y]);
						neighbors[y][x + 1].push([EDGEDIR.LEFT, EDGETYPE.LEFT, x, y]);
						break;
					case CELLPOS.RIGHT:
						neighbors[y][x + 1].push([EDGEDIR.DOWN, EDGETYPE.RIGHT, x, y]);
						neighbors[y + 1][x + 1].push([EDGEDIR.UP, EDGETYPE.LEFT, x, y]);
						break;
					case CELLPOS.BOTTOM:
						neighbors[y + 1][x + 1].push([EDGEDIR.LEFT, EDGETYPE.RIGHT, x, y]);
						neighbors[y + 1][x].push([EDGEDIR.RIGHT, EDGETYPE.LEFT, x, y]);
						break;
					case CELLPOS.LEFT:
						neighbors[y + 1][x].push([EDGEDIR.UP, EDGETYPE.RIGHT, x, y]);
						neighbors[y][x].push([EDGEDIR.DOWN, EDGETYPE.LEFT, x, y]);
						break;
					default: continue;
				}
			}
		}

		return neighbors;
	}

	// finds path from given start to given end (destroys given graph)
	// returned path is list of node indices and some other information
	// each member of returned list is array which contains: x and y coord of graph node, edge direction, edge type (of original node), x and y coord (of original node)
	function findPath(graph, startX, startY, endX, endY, initialEdgeDir) {

		var nodeX = startX, nodeY = startY;
		var nextX = startX, nextY = startY;
		var edgeType;
		var edgeDir;
		var neighbor;
		var lastEdgeDirInversed = EDGEDIR.UNKNOWN;

		var resultPath = [];
		resultPath.push([startX, startY, initialEdgeDir, EDGETYPE.UNKNOWN, -1, -1])

		while (nodeX != endX || nodeY != endY) {
			var nghbrs = graph[nodeY][nodeX];
			if (nghbrs === false || nghbrs.length > 2) {
				break;  // cycle or too many edges
			}

			for (var i = 0; i < nghbrs.length; i++) {
				if (nghbrs[i][0] == lastEdgeDirInversed) {
					continue; // we do not want to return to previous node
				}

				neighbor = nghbrs[i];
				switch (neighbor[0]) {
					case EDGEDIR.UP: nextX = nodeX; nextY = nodeY - 1; break;
					case EDGEDIR.RIGHT: nextX = nodeX + 1; nextY = nodeY; break;
					case EDGEDIR.DOWN: nextX = nodeX; nextY = nodeY + 1; break;
					case EDGEDIR.LEFT: nextX = nodeX - 1; nextY = nodeY; break;
				}
				break;  // edge found
			}

			if (nodeX == nextX && nodeY == nextY) {
				break;  // no edge found
			}

			resultPath.push([nextX, nextY, neighbor[0], neighbor[1], neighbor[2], neighbor[3]]);  // save new node to result path
			graph[nodeY][nodeX] = false;  // mark current node as visited
			lastEdgeDirInversed = inversEdge(neighbor[0]);
			nodeX = nextX;
			nodeY = nextY;
		}

		return resultPath;
	}


	function drawPath(g, path, cellWid, cellHei, closed) {
		var len = path.length;
		var arrHeadLen = (cellWid + cellHei) / 16;
		for (var i = 1; i < len; i++) {
			g.beginPath();
			g.lineWidth = 3;
			g.strokeStyle = closed ? '#070' : '#700';
			g.lineCap = 'round';
			drawArrow(g, path[i - 1][0] * cellWid + margin, path[i - 1][1] * cellHei + margin,
				path[i][0] * cellWid + margin, path[i][1] * cellHei + margin, arrHeadLen);
			g.stroke();

			var text = edgeTypeToSymbol(path[i][3]);
			g.font = 'bold 32px sans-serif';
			g.fillStyle = '#FFF';
			g.fillText(text,
				path[i][4] * cellWid + cellWid / 2 - g.measureText(text).width / 2 + margin,
				path[i][5] * cellHei + cellHei / 2 + 10 + margin);
		}
	}

	function getPath(designer) {
		var graph = convertToGraph(designer.data);
		var startY = designer.edgeType == EDGETYPE.LEFT ? designer.rows : 0;
		return findPath(graph, 0, startY, designer.cols, startY, EDGEDIR.RIGHT);
	}

	function isPathClosed(path, designer) {
		var startY = designer.edgeType == EDGETYPE.LEFT ? designer.rows : 0;
		return path[path.length - 1][0] == designer.cols && path[path.length - 1][1] == startY;
	}

	function redrawDesigner(designer) {
		var g = designer.bgCanvas.getContext('2d');
		var cols = designer.cols;
		var rows = designer.rows;

		g.clearRect(0, 0, g.canvas.width, g.canvas.height);
		drawGrid(g, cols, rows, designer.cellWid, designer.cellHei);
		drawStartAdnEnd(g, designer.edgeType, cols * designer.cellWid, rows * designer.cellHei);
		redrawCells(g, designer.data, designer.cellWid, designer.cellHei);

		var path = getPath(designer);
		drawPath(g, path, designer.cellWid, designer.cellHei, isPathClosed(path, designer));

		if (checkFass) {
			var fassData = collectFassNodeData(path, cols, rows);
			visualizeFassData(g, fassData, designer.cellWid, designer.cellHei);
		}
	}

	function collectFassNodeData(path, cols, rows) {
		var fassData = new Array(rows + 1);

		for (var y = 0; y <= rows; y++) {
			fassData[y] = new Array(cols + 1);
			for (var x = 0; x <= cols; x++) {
				fassData[y][x] = 0;
			}
		}

		var len = path.length;
		for (var i = 1; i < len; i++) {
			var x = path[i][0];
			var y = path[i][1];
			fassData[y][x]++;

			if (x == 0) {
				fassData[y][cols]++;
			}
			else if (x == cols) {
				fassData[y][0]++;
			}

			if (y == 0) {
				fassData[rows][x]++;
			}
			else if (y == rows) {
				fassData[0][x]++;
			}
		}

		// fix the first point
		fassData[path[0][1]][path[0][0]] = 1;
		// compensate neighbor of the first point
		var x = path[0][0];
		var y = path[0][1];
		if (y == 0) {
			fassData[rows][x]++;
		}
		else if (y == rows) {
			fassData[0][x]++;
		}

		return fassData;
	}

	function visualizeFassData(g, fassData, cellWid, cellHei) {

		var cols = fassData[0].length;
		var rows = fassData.length;

		for (var y = 0; y < rows; y++) {
			for (var x = 0; x < cols; x++) {
				g.beginPath();
				g.arc(x * cellWid + margin, y * cellHei + margin, 5, 0, 2 * Math.PI, false);
				if (fassData[y][x] == 0) {
					g.lineWidth = 2;
					g.strokeStyle = '#F00';
					g.stroke();
				}
				else if (fassData[y][x] == 1) {
					g.fillStyle = '#0F0';
					g.fill();
				}
				else {
					g.fillStyle = '#F00';
					g.fill();
				}
			}
		}
	}

	function getTurnSymbol(lastEdgeDir, edgeDir, rightSymbol, leftSymbol) {
		switch (lastEdgeDir) {
			case EDGEDIR.UP:
				switch (edgeDir) {
					case EDGEDIR.RIGHT: return rightSymbol;
					case EDGEDIR.LEFT: return leftSymbol;
				}
				break;
			case EDGEDIR.RIGHT:
				switch (edgeDir) {
					case EDGEDIR.UP: return leftSymbol;
					case EDGEDIR.DOWN: return rightSymbol;
				}
				break;
			case EDGEDIR.DOWN:
				switch (edgeDir) {
					case EDGEDIR.RIGHT: return leftSymbol;
					case EDGEDIR.LEFT: return rightSymbol;
				}
				break;
			case EDGEDIR.LEFT:
				switch (edgeDir) {
					case EDGEDIR.UP: return rightSymbol;
					case EDGEDIR.DOWN: return leftSymbol;
				}
				break;
		}

		return '';
	}

	function generateRewriteRule(designer, path) {

		var rewriteRule = '\trewrite ' + edgeTypeToSymbol(designer.edgeType) + ' to ';


		if (!isPathClosed(path, designer)) {
			rewriteRule += '/* path is not closed correctly! */;\n';
			return rewriteRule;
		}

		var len = path.length;
		for (var i = 1; i < len; i++) {
			rewriteRule += getTurnSymbol(path[i - 1][2], path[i][2], '- ', '+ ') + edgeTypeToSymbol(path[i][3]) + ' ';
		}

		rewriteRule += getTurnSymbol(path[len - 1][2], path[0][2], '- ', '+ ');

		rewriteRule += ';\n';
		return rewriteRule;
	}

	function generateLsystem(designers) {

		var sourceCode = 'lsystem Curve {\n' +
			'\tset symbols axiom = L;\n' +
			'\tset iterations = 3;\n' +
			'\tset interpretEveryIterationFrom = 1;\n' +
			'\n';

		$.each(designers, function (i, designer) {
			sourceCode += '\tinterpret ' + edgeTypeToSymbol(designer.edgeType) +
				' as DrawForward(' + Math.max(designer.cols, designer.rows) + ' ^ -currentIteration * ' +
					Math.round(100 + Math.sqrt((designer.cols + designer.rows) / 2) * 200) + ');\n';
		});

		sourceCode += '\tinterpret + as TurnLeft(90);\n' +
			'\tinterpret - as TurnLeft(-90);\n' +
			'\n';

		$.each(designers, function (i, designer) {
			var path = getPath(designer);
			sourceCode += generateRewriteRule(designer, path, EDGEDIR.RIGHT);
		});

		sourceCode += '}\nprocess all with SvgRenderer;';

		$('#sourceCodeResult').text(sourceCode);
	}

	function displayResult(output, container) {
		if (output.mime == 'image/svg+xml') {
			//console.log(output.url);
			$("#svgOutputTemplate").tmpl({ url: output.url, width: getValueFromMetadata('width', output.metadata), height: getValueFromMetadata('height', output.metadata) })
				.appendTo(container);
		}
	}

	function getValueFromMetadata(key, metadata) {
		$.each(metadata, function (i, item) {
			if (item[key] !== undefined) {
				return item[key]
			}
		});

		return undefined;
	}

	function clearDesignerData(data) {

		var rows = data.length;
		var cols = data[0].length;

		for (var y = 0; y < rows; y++) {
			for (var x = 0; x < cols; x++) {
				data[y][x] = CELLPOS.CENTER;
			}
		}
	}

	function mirrorDesignerData(srcData, destData) {
		var rows = srcData[0].length;
		var cols = srcData.length;

		for (var y = 0; y < rows; y++) {
			for (var x = 0; x < cols; x++) {
				var cellPos = srcData[rows - y - 1][cols - x - 1];
				destData[y][x] = inverseCellPos(cellPos);
			}
		}
	}

	function mirrorDesigners(src, dest) {
		if (!src.data || !dest.data || src.data.length != dest.data.length || src.data[0].length != dest.data[0].length) {
			return;
		}

		var rows = src.data[0].length;
		var cols = src.data.length;

		mirrorDesignerData(src.data, dest.data);

		redrawDesigner(src);
		redrawDesigner(dest);
		generateLsystem(designers);
	}


	var domDesigners = $('canvas.curveDesigner');
	//console.log(domDesigners);

	var designers = {};
	var autoMirrorElement = $('#autoMirror');
	var autoMirror = false;
	var autoMirrorSrcId = autoMirrorElement.attr('data-src-id');
	var autoMirrorDestId = autoMirrorElement.attr('data-dest-id');
	var checkFassElement = $('#checkFassRules');

	var checkFass = false;


	function autoMirror_change() {
		if (autoMirror == this.checked) {
			return;
		}

		autoMirror = this.checked;
		if (autoMirror) {
			mirrorDesigners(designers[autoMirrorSrcId], designers[autoMirrorDestId]);
		}
	}
	autoMirrorElement.change(autoMirror_change);

	function checkFass_change() {
		if (checkFass == this.checked) {
			return;
		}

		checkFass = this.checked;
		if (checkFass) {
			autoMirrorElement.attr('checked', 'checked').change();
			autoMirrorElement.disable();
		}
		else {
			autoMirrorElement.enable();
		}

		$.each(designers, function (i, designer) {
			redrawDesigner(designer);
		});
	}
	checkFassElement.change(checkFass_change);



	$.each(domDesigners, function (i, element) {
		var jqEle = $(element)
		var cols = parseInt(jqEle.attr('data-cols'));
		var rows = parseInt(jqEle.attr('data-rows'));

		var emptyData = new Array(rows);
		for (var y = 0; y < rows; y++) {
			emptyData[y] = new Array(cols);
		}

		clearDesignerData(emptyData);

		var edgeT = jqEle.attr('data-edge-type');

		designers[element.id] = {
			canvas: element,
			bgCanvas: null,
			cols: cols,
			rows: rows,
			cellWid: -1,
			cellHei: -1,
			edgeType: edgeT,
			data: emptyData
		};
	});

	//console.log(designers);

	$.each(designers, function (designerId, designer) {
		var canvas = designer.canvas;
		var wid = canvas.width;
		var hei = canvas.height;
		var cols = designer.cols;
		var rows = designer.rows;

		// creation of bg canvas
		var bgCanvas = document.createElement('canvas');
		if (!bgCanvas) {
			return;
		}

		bgCanvas.width = wid;
		bgCanvas.height = hei;

		// append bg canvas to the container
		$(canvas).parent().append(bgCanvas);
		designer.bgCanvas = bgCanvas;

		var g = canvas.getContext('2d');

		if (!g) {
			alert('Error: failed to getContext!');
			return;
		}

		var activeWid = wid - 2 * margin;
		var activeHei = hei - 2 * margin;

		var cellWid = activeWid / designer.cols;
		var cellHei = activeHei / designer.rows;

		designer.cellWid = cellWid;
		designer.cellHei = cellHei;

		redrawDesigner(designer);
		generateLsystem(designers);

		$(canvas).mousemove(function (e) {
			if (autoMirror && designerId == autoMirrorDestId) {
				return;
			}

			var pos = getMousePos(canvas, e);
			var x = pos.x, y = pos.y;
			/*if (e.layerX || e.layerX == 0) { x = e.layerX; y = e.layerY; }  // Firefox
			else if (e.offsetX || e.offsetX == 0) { x = e.offsetX; y = e.offsetY; }  // Opera*/
			x -= margin; y -= margin;

			var col = Math.floor(x / cellWid);
			var row = Math.floor(y / cellHei);
			if (col < 0 || col >= cols || row < 0 || row >= rows) {
				return;
			}

			var cellPos = getInnerCellPosition((x % cellWid) / cellWid, (y % cellHei) / cellHei);

			g.clearRect(0, 0, wid, hei);
			drawCellLine(g, cellPos, col * cellWid + margin, row * cellHei + margin, cellWid, cellHei);
		});

		$(canvas).mouseleave(function (e) {
			g.clearRect(0, 0, wid, hei);
		});

		$(canvas).mousedown(function (e) {
			if (autoMirror && designerId == autoMirrorDestId) {
				return;
			}

			var pos = getMousePos(canvas, e);
			var x = pos.x, y = pos.y;
			/*if (e.layerX || e.layerX == 0) { x = e.layerX; y = e.layerY; }  // Firefox
			else if (e.offsetX || e.offsetX == 0) { x = e.offsetX; y = e.offsetY; }  // Opera*/
			x -= margin; y -= margin;

			var col = Math.floor(x / cellWid);
			var row = Math.floor(y / cellHei);
			if (col < 0 || col >= cols || row < 0 || row >= rows) {
				return;
			}

			var cellPos = getInnerCellPosition((x % cellWid) / cellWid, (y % cellHei) / cellHei);

			designer.data[row][col] = cellPos;
			if (autoMirror) {
				designers[autoMirrorDestId].data[rows - row - 1][cols - col - 1] = inverseCellPos(cellPos);
				redrawDesigner(designers[autoMirrorDestId]);
			}

			redrawDesigner(designer);
			generateLsystem(designers);
		});

	});

	//console.log(designers);

	$('#clearCds').click(function (e) {
		e.preventDefault();
		$.each(designers, function (i, designer) {
			clearDesignerData(designer.data);
			redrawDesigner(designer);
		});
		generateLsystem(designers);
	});

	$('.curveDesigneMirrors>a.mirror').click(function (e) {
		e.preventDefault();
		var srcId = $(this).attr('data-src-id');
		var destId = $(this).attr('data-dest-id');

		mirrorDesigners(designers[srcId], designers[destId]);
	});

	$('#process').click(function (e) {
		var srcCode = $('#sourceCodeResult').text();
		var postData = { sourceCode: srcCode };

		$.ajax({
			type: 'POST',
			url: '/api/process',
			data: postData,
			beforeSend: function () {
				$('#resultsContainer').text("");
				$('#ajaxLoader').show();
			},
			success: function (recvData) {
				//console.log(recvData);
				var container = $('#resultsContainer');
				$.each(recvData.outputs, function (i, output) {
					displayResult(output, container);
				});
			},
			complete: function () {
				$('#ajaxLoader').hide();
			}
		});
	});

	$('#edit').click(function (e) {
		$('#SourceCode').attr('value', $('#sourceCodeResult').text());
	});

	autoMirrorElement.change();
	checkFassElement.change();

} (jQuery));

