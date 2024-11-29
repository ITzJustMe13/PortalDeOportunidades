import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/PaginatedOpportunityGallery.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';

class SearchPage extends StatefulWidget {
  const SearchPage({super.key});

  @override
  State<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {

  var _isDrawerOpen = false;

  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  String? _keyword;
  Location? _location;
  OppCategory? _category;
  int? _vacancies;
  int? _minPrice;
  int? _maxPrice;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      appBar:CustomAppBar(
        bottom: PreferredSize(
          preferredSize: Size.fromHeight(50.0),
          child: Container(
            color: Color(0xFFD9D9D9),
            child: Padding(
              padding: EdgeInsets.symmetric(horizontal: 8.0),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  IconButton(
                      icon: Icon(Icons.search),
                      onPressed: () {
                        setState(() {
                          _isDrawerOpen = !_isDrawerOpen;
                        });
                      }),
                  TextButton(
                      onPressed: null,
                      style: TextButton.styleFrom(
                          padding: EdgeInsets.zero,
                          backgroundColor: Color(0xFF50C878)),
                      child: Text("TOP",
                          style: TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.bold))),
                ],
              ),
            ),
          ),
        ),
      ),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          double screenWidth = constraints.maxWidth;

          var isMobile = screenWidth < 600;
          var isTablet = 600 <= screenWidth && screenWidth < 1200;

          double drawerWidth = 0;

          if (isMobile) {
            drawerWidth = screenWidth * 1.0;
          } else if (isTablet) {
            drawerWidth = screenWidth / 3;
          } else {
            drawerWidth = screenWidth / 5;
          }

          return SingleChildScrollView(
            // Make the entire body scrollable
            child: isMobile
                ? Column(
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (_isDrawerOpen) _buildDrawer(drawerWidth, isMobile),
                      Padding(
                        padding: EdgeInsets.symmetric(horizontal: 8.0),
                        child: _buildGallery(),
                      ),
                    ],
                  )
                : Row(
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      if (_isDrawerOpen) _buildDrawer(drawerWidth, isMobile),
                      Expanded(
                        child: Padding(
                          padding: EdgeInsets.symmetric(horizontal: 8.0),
                          child: _buildGallery(),
                        ),
                      ),
                    ],
                  ),
          );
        },
      ),
    );
  }

  Widget _buildGallery() {
    return Column(
      children: [
        SizedBox(height: 24),
        /*
        PaginatedOpportunityGallery(
          allOpportunities: opportunitiesList,
        )*/
      ],
    );
  }

  Widget _buildDrawer(double width, bool isMobile) {
    return Container(
      color: isMobile ? Color(0xFFD9D9D9) : null,
      width: width,
      child: SingleChildScrollView(
        // Make the drawer scrollable if its height exceeds screen
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child:
              Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
            Text(
              "Search & Filter",
              style: TextStyle(fontSize: 20),
            ),
            Divider(),
            TextField(
              decoration: InputDecoration(
                labelText: 'Keyword',
                border: OutlineInputBorder(),
              ),
              onChanged: (text) {
                setState(() {
                  _keyword = text;
                });
              },
            ),
            SizedBox(height: 8),
            TextField(
              keyboardType: TextInputType.number,
              inputFormatters: [
                FilteringTextInputFormatter.digitsOnly,
              ],
              decoration: InputDecoration(
                labelText: 'Vacancies',
                border: OutlineInputBorder(),
              ),
              onChanged: (text) {
                setState(() {
                  _vacancies = int.parse(text);
                });
              },
            ),
            SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Flexible(
                  child: TextField(
                    keyboardType: TextInputType.number,
                    inputFormatters: [
                      FilteringTextInputFormatter.digitsOnly,
                    ],
                    decoration: InputDecoration(
                      labelText: 'Min Price',
                      border: OutlineInputBorder(),
                    ),
                    onChanged: (text) {
                      setState(() {
                        _minPrice = int.parse(text);
                      });
                    },
                  ),
                ),
                const SizedBox(width: 8),
                Flexible(
                  child: TextField(
                    keyboardType: TextInputType.number,
                    inputFormatters: [
                      FilteringTextInputFormatter.digitsOnly,
                    ],
                    decoration: InputDecoration(
                      labelText: 'Max Price',
                      border: OutlineInputBorder(),
                    ),
                    onChanged: (text) {
                      setState(() {
                        _maxPrice = int.parse(text);
                      });
                    },
                  ),
                ),
              ],
            ),
            SizedBox(height: 8),
            DropdownButton<OppCategory>(
              value: _category,
              hint: Text("Select Category"),
              onChanged: (OppCategory? newValue) {
                setState(() {
                  _category = newValue;
                });
              },
              isExpanded: true, // Make the dropdown button take full width
              items: OppCategory.values.map((OppCategory category) {
                return DropdownMenuItem<OppCategory>(
                  value: category,
                  child: Text(
                    category.name.replaceAll(
                        '_', ' '), // Replace underscores with spaces
                    overflow: TextOverflow
                        .ellipsis, // Truncate with ellipsis if too long
                    maxLines: 2, // Ensure only one line of text
                  ),
                );
              }).toList(),
              selectedItemBuilder: (BuildContext context) {
                return OppCategory.values.map((OppCategory category) {
                  return Text(
                    category.name.replaceAll('_', ' '),
                    overflow: TextOverflow
                        .ellipsis, // Truncate with ellipsis for the selected item
                    maxLines:
                        1, // Ensure only one line of text for the selected value
                  );
                }).toList();
              },
            ),
            SizedBox(height: 8),
            DropdownButton<Location>(
              value: _location,
              hint: Text("Select Location"),
              onChanged: (Location? newValue) {
                setState(() {
                  _location = newValue;
                });
              },
              isExpanded: true, // Make the dropdown button take full width
              items: Location.values.map((Location location) {
                return DropdownMenuItem<Location>(
                  value: location,
                  child: Text(
                    location.name.replaceAll(
                        '_', ' '), // Replace underscores with spaces
                    overflow: TextOverflow
                        .ellipsis, // Truncate with ellipsis if too long
                    maxLines: 2, // Ensure only one line of text
                  ),
                );
              }).toList(),
              selectedItemBuilder: (BuildContext context) {
                return Location.values.map((Location location) {
                  return Text(
                    location.name.replaceAll('_', ' '),
                    overflow: TextOverflow
                        .ellipsis, // Truncate with ellipsis for the selected item
                    maxLines:
                        1, // Ensure only one line of text for the selected value
                  );
                }).toList();
              },
            ),
            SizedBox(height: 8),
            Center(
              child: TextButton(
                  onPressed: null,
                  style: TextButton.styleFrom(
                      padding: EdgeInsets.zero,
                      backgroundColor: Color(0xFF50C878)),
                  child: Text("Filter")),
            )
          ]),
        ),
      ),
    );
  }
}
