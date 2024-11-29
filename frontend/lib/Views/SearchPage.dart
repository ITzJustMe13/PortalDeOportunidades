import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Components/PaginatedOpportunityGallery.dart';
import 'package:frontend/Components/SearchDrawer.dart';
import 'package:frontend/State/SearchState.dart';
import 'package:provider/provider.dart';

class SearchPage extends StatelessWidget {
  SearchPage({super.key});

  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  @override
  Widget build(BuildContext context) {
    return Consumer<SearchState>(builder: (context, searchState, child) {
      return Scaffold(
        key: _scaffoldKey,
        appBar: CustomAppBar(
          bottom: _buildBottomBar(searchState),
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
              child: isMobile
                  ? Column(
                      mainAxisAlignment: MainAxisAlignment.start,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (searchState.isDrawerOpen)
                          SearchDrawer(width: drawerWidth, isMobile: isMobile),
                        Padding(
                          padding: EdgeInsets.symmetric(horizontal: 8.0),
                          child: _buildGallery(searchState),
                        ),
                      ],
                    )
                  : Row(
                      mainAxisAlignment: MainAxisAlignment.start,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (searchState.isDrawerOpen)
                          SearchDrawer(width: drawerWidth, isMobile: isMobile),
                        Expanded(
                          child: Padding(
                            padding: EdgeInsets.symmetric(horizontal: 8.0),
                            child: _buildGallery(searchState),
                          ),
                        ),
                      ],
                    ),
            );
          },
        ),
      );
    });
  }

  PreferredSize _buildBottomBar(SearchState searchState) {
    return PreferredSize(
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
                    searchState.updateIsDrawerOpen();
                  }),
              SizedBox(width: 24),
              DynamicActionButton(
                onPressed: () => searchState.toggleTopChecked(),
                icon: Icons.list,
                text: searchState.isTopChecked ? "TOP" : "All",
                color: Color(0xFF50C878),
              ),
              SizedBox(width: 24),
              Flexible(
                child: Container(
                  constraints: BoxConstraints(maxWidth: 300), // Set max width
                  child: DropdownButton<String>(
                    value: searchState.selectedSort,
                    hint: Text('Escolhe o tipo de ordenação'),
                    isExpanded: true,
                    onChanged: (String? newValue) async {
                      searchState.setSelectedSort(newValue);

                      if (newValue != null &&
                          searchState.sortOptions[newValue] != null) {
                        // Call the selected sort method
                        await searchState.sortOptions[newValue]!(searchState);
                      }
                    },
                    items:
                        searchState.sortOptions.keys.map((String sortOption) {
                      return DropdownMenuItem<String>(
                        value: sortOption,
                        child: Text(sortOption),
                      );
                    }).toList(),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildGallery(SearchState searchState) {
    return searchState.isLoading
        ? Column(children: [
            SizedBox(height: 24),
            Center(
              child: CircularProgressIndicator(),
            ),
            SizedBox(height: 24),
            Text(
              "A carregar...",
            ),
          ])
        : Column(
            children: [
              SizedBox(height: 24),
              PaginatedOpportunityGallery(
                allOpportunities: searchState.opportunitiesList ?? [],
              )
            ],
          );
  }
}
